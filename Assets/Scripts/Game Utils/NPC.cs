using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ADD 'SpaceShip' CLASS HERE...

public class NPC : MonoBehaviour
{


    [SerializeField] private int _systemID;
    [SerializeField] private int _npcID = -1; // Unique identifier for NPCs. Default value is -1, means empty
    [SerializeField] private int _credits;

    public string race;

    // bottom of them will be cleaned
    
    public Objective objective;
    public Spaceship ship;

    public GameObject player;

    public float patrolRange = 10f;

    private Vector2 _currentLoc;
    [SerializeField] private GameObject _targetObj;
    [SerializeField] private Vector2 _targetLoc;
    private Vector2 _dir;
    private float _angle;

    private Vector2[] directions;
    private int index;
    private GameManager _gm;

    [SerializeField] private NpcManager _npcManager;

    private Vector3 _nullVector = new Vector3(0, 0, -1);
    [SerializeField] private int remainingWandering;



    // PORTAL SEEK REALM
    public bool teleportReady = false;

    private readonly int _wanderingLimit = 2;

    void Awake()
    {
        _gm = GameManager.Instance;

        FillRemainingWandering();
        _currentLoc = (Vector2) transform.position;
        // DEFINITELY CHANGE IT
        _targetLoc = _currentLoc; // exceptional situation, just for initialization

        if(player == null)
        {
            // Debug.Log("Finding Player");
            player = GameObject.Find("Player");
        }

        directions = new Vector2[4]{ Vector2.left, Vector2.up, Vector2.right, Vector2.down};
        index = 0;

    }

    // DONE WHEN SPAWNING, DON'T WORRY
    public void SetNpcManager(NpcManager mng)
    {
        _npcManager = mng;
    }

    public void NpcFakeUpdate() // for making the npcs move when disabled
    {
        _currentLoc = (Vector2) transform.position;
        HandleObjective();

    }

    /*void Update() // update will not work when disabled, change it at some point
    {
        _currentLoc = (Vector2) transform.position;
        HandleObjective();
    }*/

    // NOTE: Couldn't fix the problem where "npcID = 0" at the start. This method is unsafe, be careful...
    public void SetNPCID(int num) // -1 is the default value
    {
        _npcID = num;
    }

    public int GetNPCID()
    {
        return _npcID;
    }

    public void SetSystemID(int num)
    {
        _systemID = num;
    }

    public int GetSystemID()
    {
        return _systemID;
    }

    public void SetNpcCredits(int num)
    {
        _credits = num;
    }

    public int GetNpcCredits()
    {
        return _credits;
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
            
            case Objective.SeekPlanet:
                HandleSeekPlanet();
                break;

            default:
                throw new MissingComponentException("Shit sn");
        }
        HandleMovement();
    }

    private void OnSeekPlanetActivated()
    {
        GameObject closestPlanet = _npcManager.GetClosestPlanetObjByPos(_systemID, transform.position);

        SetTarget(closestPlanet);
    }

    bool CheckSwitchFromRandomWandering(Objective newObjective)
    {
        if(--remainingWandering < 0)
        {
            SetObjective(newObjective);
            return true;
        }
        return false;
    }

    void HandleRandomWandering()
    {
        // TODO: DEAL WITH HARD CODED ELEMENTS SUCH AS WANDERING RANGE
        int lim = 100;
        if(IsTargetReached())
        {
            if(CheckSwitchFromRandomWandering(Objective.SeekPlanet)){return;}
            Vector2 vec = new Vector2(Random.Range(-lim, lim), Random.Range(-lim, lim));
            SetTarget(vec);
        }
    }

    void SetObjective(Objective newObjective)
    {
        if(newObjective == Objective.SeekPlanet)
            OnSeekPlanetActivated();
        objective = newObjective;
    }

    void HandleFollow()
    {
        _targetLoc = (Vector2) player.transform.position;
    }

    void FillRemainingWandering()
    {
        remainingWandering = _wanderingLimit;
    }

    bool IsTargetReached()
    {
        return (_currentLoc == _targetLoc) ? true : false;
    }

    bool IsTargetReached(int permittedDistance)
    {
        return Vector2.Distance(_currentLoc, _targetLoc) <= permittedDistance;
    }

    void HandlePatrol()
    {
        if(IsTargetReached())
        {
            SetPatrolTarget(directions[index%(directions.Length)]);
            index++;
        }
    }

    GameObject ClosestObjectByTag(string tag)
    {
        GameObject chosenObject = null;

        GameObject[] objects;
        objects = GameObject.FindGameObjectsWithTag(tag);
        if(objects.Length == 0)
        {
            Debug.LogWarning("NO " + tag + " AVAILABLE, BEWARE");
            return null;
        }
        float closestPos = -1f;
        float temp;
        foreach (GameObject obj in objects)
        {
            temp = _GetDistanceToPos((Vector2)obj.transform.position);
            if(temp < closestPos || closestPos == -1f)
            {
                closestPos = temp;
                chosenObject = obj;
            }
        }
        return chosenObject;
    }

    void HandleSeekPortal()
    {
        // BURADA HİÇ PORTAL YOKSA OLACAKLARI DÜŞÜN
        if(!teleportReady)
        {
            // I MIGHT HAVE TO CHANGE THIS BECAUSE PORTAL USAGE IS DIFFERENT NOW
            // I HAVEN'T DECIDED WHETHER I'M GONNA PUT EVERY POSSIBLE PORTAL IN ONE SCENE OR ELSE
            GameObject closestPortal = ClosestObjectByTag("Portal");
            // Vector3 portalLoc = closestPortal.transform.position;
            SetTarget(closestPortal);
            teleportReady = true;
        }
    }

    void Buy(int shopId, int itemId)
    {
        _npcManager.NpcBuyItem(shopId, itemId, _npcID);
    }

    void HandleSeekPlanet() // CHANGES ITS FOCUS WHENEVER A PLANET IS CLOSER TO IT (ADHD)
    {
        
        if(IsTargetReached(5))
        {
            // 5 IS THE TEST ITEM!!!!!
            Buy(_targetObj.GetComponent<Planet>().GetShopID(), 5);

            FillRemainingWandering();
            SetObjective(Objective.RandomWandering);
        }
    }

    private float _GetDistanceToPos(Vector2 loc)
    {
        return (loc - (Vector2) transform.position).magnitude;
    }

    public void SetTarget(GameObject obj)
    {
        _targetObj = obj;
        _targetLoc = obj.transform.position;
    }

    public void SetTarget(Vector2 loc)
    {
        _targetObj = null;
        _targetLoc = loc;
    }

    public Vector2 SetPatrolTarget(Vector2 direction)
    {
        _targetLoc = _currentLoc + (direction.normalized * patrolRange); // (Vector2) player.transform.position; 
        Debug.Log("Target: " + _targetLoc);
        return _targetLoc;
    }

    public Vector2 HandleMovement()
    {
        transform.position = Vector2.MoveTowards(_currentLoc, _targetLoc, Time.deltaTime * ship.speed);
        _dir = _targetLoc - _currentLoc;
        _angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(_angle - 90, Vector3.forward);
        return transform.position;
    }
}
