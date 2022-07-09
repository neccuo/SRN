using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Objective
{
    Follow,
    Patrol,
    SeekPortal,
    RandomWandering,
    SeekPlanet
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
    private GameManager _gm;

    private Vector3 _nullVector = new Vector3(0, 0, -1);


    // PORTAL SEEK REALM
    public bool teleportReady = false;
    private GameObject _chosenObject;

    private readonly int _wanderingLimit = 2;
    [SerializeField] private int remainingWandering;

    void Start()
    {
        // objective = Objective.Patrol; // starts with patrolling for testing

        _gm = GameManager.Instance;

        FillRemainingWandering();
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
            
            case Objective.SeekPlanet:
                HandleSeekPlanet();
                break;

            default:
                throw new MissingComponentException("Shit sn");
        }
        HandleMovement();
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
        objective = newObjective;
    }

    void HandleFollow()
    {
        _target = (Vector2) player.transform.position;
    }

    void FillRemainingWandering()
    {
        remainingWandering = _wanderingLimit;
    }

    bool IsTargetReached()
    {
        return (_currentLoc == _target) ? true : false;
    }

    bool IsTargetReached(int permittedDistance)
    {
        return (Vector2.Distance(_currentLoc, _target) <= permittedDistance) ? true : false;
    }

    void HandlePatrol()
    {
        if(IsTargetReached())
        {
            SetPatrolTarget(directions[index%(directions.Length)]);
            index++;
        }
    }

    Vector3 ClosestObjectLocationByTag(string tag)
    {
        GameObject[] objects;
        objects = GameObject.FindGameObjectsWithTag(tag);
        if(objects.Length == 0)
        {
            Debug.LogWarning("NO " + tag + " AVAILABLE, BEWARE");
            return _nullVector;
        }
        float closestPos = -1f;
        float temp;
        foreach (GameObject obj in objects)
        {
            temp = _GetDistanceToPos((Vector2)obj.transform.position);
            if(temp < closestPos || closestPos == -1f)
            {
                closestPos = temp;
                _chosenObject = obj;
            }
        }
        return _chosenObject.transform.position;
    }

    void TargetClosestObjectByTag(string tag)
    {
        Vector3 closestObject = ClosestObjectLocationByTag(tag);
        if(closestObject == _nullVector){return;}
        SetTarget((Vector2)closestObject);
    }

    void HandleSeekPortal()
    {
        // BURADA HİÇ PORTAL YOKSA OLACAKLARI DÜŞÜN
        if(!teleportReady)
        {
            TargetClosestObjectByTag("Portal");
            teleportReady = true;
        }
    }

    void BuyItem()
    {
        int randInt = Random.Range(0, 3);
        int randInt2 = Random.Range(1, 4);
        // StartCoroutine(_gm.saveLoad.NpcItemBuy(npcID, randInt, randInt2));
    }

    void SellItem()
    {
        int randInt = Random.Range(0, 3);
        int randInt2 = Random.Range(1, 2); // returns 1
        // StartCoroutine(_gm.saveLoad.NpcItemSell(npcID, randInt, randInt2));
    }

    void HandleSeekPlanet() // CHANGES ITS FOCUS WHENEVER A PLANET IS CLOSER TO IT (ADHD)
    {
        TargetClosestObjectByTag("Planet");
        if(IsTargetReached(5))
        {
            int randInt = Random.Range(0, 2);
            if(randInt == 0) BuyItem(); else SellItem();
            FillRemainingWandering();
            SetObjective(Objective.RandomWandering);
        }
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
