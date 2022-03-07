using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // export realm
    public Spaceship ship;
    public HealthBar hpBar;
    public Camera cam;
    public CreditManager creditManager;

    private GameObject _followedObject;

    // cam realm
    private float _minimumZoom = 10;
    private float _zoomUnit = 1;

    // movement realm
    private Vector2 _target;
    private Vector2 _dir;
    private float _angle;

    // for getting game state
    private GameManager currentGameInstance;

    void Start()
    {
        // initializing game
        if(hpBar)
            hpBar.SetMaxHealth(ship.maxHealth);

        _target = transform.position;
        _angle = transform.rotation.z;

        currentGameInstance = GameManager.Instance;
    }

    void Update()
    {
        HandleDamageInputs();
        // HandleGameState();
    }

    /*void HandleGameState()
    {
        switch (currentGameInstance.state)
        {
            case GameState.PlanMovement:
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
                {
                    SetMovement();
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    currentGameInstance.UpdateGameState(GameState.DuringMovement);
                }
                if(Input.GetKeyDown(KeyCode.Alpha9))
                {
                    HandleCamZoom(-1);
                }
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    HandleCamZoom(1);
                }
                break;
            case GameState.DuringMovement:

                HandleMovement();
                if ((Vector2)transform.position == _target)
                {
                    currentGameInstance.UpdateGameState(GameState.PlanMovement);
                }
                break;
            case GameState.TurnEvaluation:
                break;
            case GameState.Combat:
                break;
            default:
                throw new MissingComponentException("" + currentGameInstance.ToString() + "is not an available state.");
        }
    }*/

    public void ResetTarget()
    {
        _target = transform.position;
    }

    public Vector2 GetTarget()
    {
        return _target;
    }

    public void SetFollowedObject(GameObject obj)
    {
        _followedObject = obj;
    }

    public GameObject GetFollowedObject()
    {
        return _followedObject;
    }

    public Vector2 SetMovementBasic()
    {
        SetFollowedObject(null);
        _target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        _angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(_angle - 90, Vector3.forward);
        return _target;
    }

    public Vector2 SetMovementFollow()
    {
        // NOTHING TO DO WITH THE CAMERA
        _target = _followedObject.transform.position;
        _dir = _followedObject.transform.position - transform.position;
        _angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(_angle - 90, Vector3.forward);
        return _target;
    }

    public Vector2 HandleMovement()
    {
        transform.position = Vector2.MoveTowards(transform.position, _target, Time.deltaTime * ship.speed);
        return transform.position;
    }

    void HandleCamZoom(int zoomDensity)
    {
        if(_minimumZoom <= cam.orthographicSize + (zoomDensity*_zoomUnit))
        {
            cam.orthographicSize += zoomDensity * _zoomUnit;
            Debug.LogFormat("Cam size: {0}", cam.orthographicSize);
        }
        else
        {
            Debug.LogWarning("Minimum zoom cannot be exceeded");
        }
    }

    void HandleDamageInputs() 
    {
        // MAKE SURE DAMAGE INFLICTS AT THE END OF "DuringMovement"
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            //creditManager.ExchangeCredits(-100);
            ship.TakeDamage(20);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //creditManager.ExchangeCredits(+100);
            ship.RepairDamage(17);
        }

        if(hpBar)
            hpBar.SetHealth(ship.currentHealth);
    }
}
