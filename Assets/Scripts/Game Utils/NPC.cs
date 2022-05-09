using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Objective
{
    Follow,
    Patrol,
    SeekPortal,
    RandomWandering
}

public class NPC : MonoBehaviour
{
    [SerializeField]
    private int npcID = -1; // Unique identifier for NPCs. Default value is -1, means empty
    public Objective objective;
    public Spaceship ship;

    public GameObject player;

    public float patrolRange = 10f;

    private Vector2 _currentLoc;
    private Vector2 _target;
    private Vector2 _dir;
    private float _angle;

    private Vector2[] directions;
    private int index;
    private GameManager currentGameInstance;


    // PORTAL SEEK REALM
    public bool teleportReady = false;
    private GameObject _chosenPortal;

    void Start()
    {
        // objective = Objective.Patrol; // starts with patrolling for testing

        _currentLoc = (Vector2) transform.position;
        // DEFINITELY CHANGE IT
        _target = _currentLoc; // exceptional situation, just for initialization

        if(player == null)
        {
            // Debug.Log("Finding Player");
            player = GameObject.Find("Player");
        }

        directions = new Vector2[4]{ Vector2.left, Vector2.up, Vector2.right, Vector2.down};
        index = 0;
        
    }

    void Update()
    {
        _currentLoc = (Vector2) transform.position;
        HandleObjective();
    }

    // NOTE: Couldn't fix the problem where "npcID = 0" at the start. This method is unsafe, be careful...
    public void SetNPCID(int num) // -1 is the default value
    {
        npcID = num;
        /*if(npcID == -1)
        {
            npcID = num;
            return;
        }
        Debug.LogError("Cannot assign another NPCID to an NPC");*/
    }

    public int GetNPCID()
    {
        return npcID;
    }

    void HandleObjective()
    {
        switch(objective)
        {
            case Objective.Follow:
                HandleFollow();
                break;

            case Objective.Patrol:
                HandlePatrol();
                break;

            case Objective.SeekPortal:
                HandleSeekPortal();
                break;

            case Objective.RandomWandering:
                HandleRandomWandering();
                break;

            default:
                throw new MissingComponentException("Shit sn");
        }
    }

    void HandleRandomWandering()
    {
        // TODO: DEAL WITH HARD CODED ELEMENTS SUCH AS WANDERING RANGE
        int lim = 100;
        if(_target == _currentLoc)
        {
            Vector2 vec = new Vector2(Random.Range(-lim, lim), Random.Range(-lim, lim));
            SetTarget(vec);
        }
        HandleMovement();
    }

    void HandleFollow()
    {
        _target = (Vector2) player.transform.position;
        HandleMovement();
    }

    void HandlePatrol()
    {
        if(_currentLoc == _target)
        {
            SetPatrolTarget(directions[index%(directions.Length)]);
            index++;
        }
        HandleMovement();
    }

    void HandleSeekPortal()
    {
        // BURADA HİÇ PORTAL YOKSA OLACAKLARI DÜŞÜN
        if(!teleportReady)
        {
            GameObject[] portals;
            portals = GameObject.FindGameObjectsWithTag("Portal");
            if(portals.Length == 0)
            {
                Debug.LogWarning("NO PORTAL AVAILABLE, BEWARE");
                return;
            }
            float closestPos = -1f;
            float temp;
            foreach (GameObject portal in portals)
            {
                temp = _GetDistanceToPos((Vector2)portal.transform.position);
                if(temp < closestPos || closestPos == -1f)
                {
                    closestPos = temp;
                    _chosenPortal = portal;
                }
            }
            SetTarget((Vector2)_chosenPortal.transform.position);
            teleportReady = true;
        }
        HandleMovement();

    }

    private float _GetDistanceToPos(Vector2 loc)
    {
        return (loc - (Vector2) transform.position).magnitude;
    }

    public void SetTarget(Vector2 loc)
    {
        _target = loc;
    }

    public Vector2 SetPatrolTarget(Vector2 direction)
    {
        _target = _currentLoc + (direction.normalized * patrolRange); // (Vector2) player.transform.position; 
        Debug.Log("Target: " + _target);
        return _target;
    }

    public Vector2 HandleMovement()
    {
        transform.position = Vector2.MoveTowards(_currentLoc, _target, Time.deltaTime * ship.speed);
        _dir = _target - _currentLoc;
        _angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(_angle - 90, Vector3.forward);
        return transform.position;
    }
}
