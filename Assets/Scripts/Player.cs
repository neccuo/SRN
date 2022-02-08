using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // export realm
    public Spaceship ship;
    public HealthBar hpBar;
    public Camera cam;

    // cam realm
    private float minimumZoom = 10;
    private float zoomUnit = 1;

    // movement realm
    private Vector2 target;
    private Vector2 dir;
    private float angle;

    // for getting game state
    private GameManager currentGameInstance;

    void Start()
    {
        // initializing game
        hpBar.SetMaxHealth(ship.maxHealth);

        target = transform.position;
        angle = transform.rotation.z;

        currentGameInstance = GameManager.Instance;
    }

    void Update()
    {
        HandleDamageInputs();
        HandleGameState();
    }

    void HandleGameState()
    {
        switch (currentGameInstance.State)
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
                if ((Vector2)transform.position == target)
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
    }

    void SetMovement()
    {
        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    void HandleCamZoom(int zoomDensity)
    {
        if(minimumZoom <= cam.orthographicSize + (zoomDensity*zoomUnit))
        {
            cam.orthographicSize += zoomDensity * zoomUnit;
            Debug.LogFormat("Cam size: {0}", cam.orthographicSize);
        }
        else
        {
            Debug.LogWarning("Minimum zoom cannot be exceeded");
        }
    }

    void HandleMovement()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * ship.speed);
    }

    void HandleDamageInputs()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ship.TakeDamage(20);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ship.RepairDamage(17);
        }
        hpBar.SetHealth(ship.currentHealth);
    }
}
