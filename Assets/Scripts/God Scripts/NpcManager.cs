using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcManager : MonoBehaviour
{
    [SerializeField] private SystemDB _systemDB;
    [SerializeField] private PlanetManager _planetManager;
    [SerializeField] private GameObject _npcPrefab;

    public static int npcSpawned = 0;

    private GameObject _npcObjectPointer;

    private Time time;

    private float _timer;

    private GameManager _gm;

    [SerializeField] private Dictionary<int, NPC> _npcDic = new Dictionary<int, NPC>();

    // it receives a tick and works

    void Start()
    {
        _gm = GameManager.Instance;
        _timer = 0.0f;
        if(_npcPrefab.tag != "NPC")
        {
            Debug.LogError("NPC tag is not NPC, blueprint is flawed.");
        }
    }

    void Update()
    {
        foreach(KeyValuePair<int, NPC> pair in _npcDic)
        {
            pair.Value.NpcFakeUpdate();
        }

        _timer += Time.deltaTime;
        if(_timer > 1.5f && _planetManager.transform.childCount > 0)
        {
            // DefineFreshNpc();
            _timer = 0.0f;
        }
        if(GameManager.Instance.GetCurrentState() == GameState.PlanMovement)
        {
            //if(Input.GetKeyDown(KeyCode.M)){DefineFreshNpc();}
            //else if(Input.GetKeyDown(KeyCode.N)){StartCoroutine(_gm.saveLoad.LoadDB());}
        }
    }

    void MoveNpcs()
    {

    }

    // use it at the start
    public void InitializeAllPilots()
    {
        

    }

    public void PrepareSystemNPCs(int oldSysID, int newSysID)
    {
        List<int> deactivateIdList = _systemDB.GetPilotsBySystemID(oldSysID);
        List<int> activateIdList = _systemDB.GetPilotsBySystemID(newSysID);

        foreach(int i in deactivateIdList)
        {
            _npcDic[i].gameObject.SetActive(false);
        }
        foreach(int i in activateIdList)
        {
            _npcDic[i].gameObject.SetActive(true);
        }

    }

    public Dictionary<int, NPC> GetNpcDic()
    {
        return _npcDic;
    }

    private void FillNpcDic(int id, NPC npcObj)
    {
        _npcDic[id] = npcObj;
    }

    public void SpawnNPC(PilotTEMP inp)
    {
        GameObject obj = Instantiate(_npcPrefab);
        NPC npcLogic = obj.GetComponent<NPC>();
        Transform tr = obj.transform;
        Spaceship ship = obj.GetComponentInChildren<Spaceship>();

        obj.SetActive(false);
        obj.name = inp.name;
        _RegisterHull(obj, inp.hull_id);

        tr.parent = this.transform;
        tr.position = new Vector3(inp.x_axis, inp.y_axis, 0);
        tr.Rotate(tr.rotation.x, tr.rotation.y, inp.angle); // 0s might be problematic, but anyway.

        tr.localScale = new Vector3(0.5f, 0.5f, 1); // change

        npcLogic.SetNPCID(inp.id);
        npcLogic.SetSystemID(inp.system_id);
        npcLogic.SetNpcCredits(inp.credits);
        npcLogic.race = inp.race;
        npcLogic.objective = Objective.RandomWandering; // change
        npcLogic.patrolRange = 10; //  change

        ship.maxHealth = 100; // change
        ship.maxHealth = 100; // change
        ship.speed = 25; // change

        FillNpcDic(inp.id, npcLogic);
    }

    public Vector3 PickRandomPlanetPos()
    {
        int childCount = _planetManager.transform.childCount;
        if(childCount == 0)
        {
            Debug.LogError("Planet count is 0, can't pick a planet");
            return new Vector3(0,0,-100);
        }
        int rndNum = Random.Range(0, childCount);
        Transform chosenPlanet = _planetManager.transform.GetChild(rndNum);
        Debug.Log(chosenPlanet.name + " is chosen. Will spawn in: " + chosenPlanet.position);
        return chosenPlanet.position;
    }

    private int _PickRandomShipID()
    {
        return Random.Range(1, 8);
    }

    private void _RegisterHull(SpriteRenderer sr, Spaceship ship, int id)
    {
        sr.sprite = Resources.Load<Sprite>("Sprites/Ships/spaceship-" + id.ToString());
        ship.SetHullID(id);
    }

    private void _RegisterHull(GameObject obj, int id)
    {
        SpriteRenderer sr = obj.GetComponentInChildren<SpriteRenderer>();
        Spaceship ship = obj.GetComponentInChildren<Spaceship>();
        sr.sprite = Resources.Load<Sprite>("Sprites/Ships/spaceship-" + id.ToString());
        ship.SetHullID(id);
    }

    public void LoadExistingNpc(int id, string name, int ship_id, float x_axis, float y_axis)
    {
        // let's say a tick just came and you have to set the attributes of the npc

        // POINTER ASSIGNMENT REALM
        _npcObjectPointer = _npcPrefab;

        // TRANSFORM ASSIGNMENT REALM
        Transform transformRef = _npcObjectPointer.transform; // EXPERIMENTAL

        Vector3 loc = new Vector3(x_axis, y_axis, 0);
        transformRef.position = loc; // temp
        transformRef.localScale = new Vector3(0.5f, 0.5f, 1);

        // NPC SCRIPT REALM
        NPC npcRef = _npcObjectPointer.GetComponent<NPC>();
        npcRef.SetNPCID(id);
        npcRef.objective = Objective.RandomWandering;
        npcRef.patrolRange = 10;

        // SPACESHIP REALM
        SpriteRenderer spriteRendererRef = _npcObjectPointer.GetComponentInChildren<SpriteRenderer>();
        Spaceship spaceship = _npcObjectPointer.GetComponentInChildren<Spaceship>();
        _RegisterHull(spriteRendererRef, spaceship, ship_id);

        spaceship.maxHealth = 100;
        spaceship.currentHealth = 100;
        spaceship.speed = 25;

        _npcObjectPointer = Instantiate(_npcObjectPointer);
        _npcObjectPointer.transform.SetParent(this.transform);
        _npcObjectPointer.name = name;

        Debug.Log("SPAWNED SHIP: " + _npcObjectPointer.name + " @ " + _npcObjectPointer.transform.position.ToString());

    }

    public void DefineFreshNpc()
    {
        // let's say a tick just came and you have to set the attributes of the npc

        // POINTER ASSIGNMENT REALM
        _npcObjectPointer = _npcPrefab;

        // TRANSFORM ASSIGNMENT REALM
        Transform transformRef = _npcObjectPointer.transform; // EXPERIMENTAL

        Vector3 loc = PickRandomPlanetPos(); // ASSUMING WE ARE WORKING WITH PLANETS NOW
        transformRef.position = loc; // temp
        transformRef.localScale = new Vector3(0.5f, 0.5f, 1);

        // NPC SCRIPT REALM
        NPC npcRef = _npcObjectPointer.GetComponent<NPC>();
        npcRef.objective = Objective.RandomWandering;
        npcRef.patrolRange = 10;
        Debug.Log("Set targt falan");

        // SPACESHIP REALM
        SpriteRenderer spriteRendererRef = _npcObjectPointer.GetComponentInChildren<SpriteRenderer>();
        Spaceship spaceship = _npcObjectPointer.GetComponentInChildren<Spaceship>();
        _RegisterHull(spriteRendererRef, spaceship, _PickRandomShipID());

        spaceship.maxHealth = 100;
        spaceship.currentHealth = 100;
        spaceship.speed = 25;


        _npcObjectPointer = Instantiate(_npcObjectPointer);
        _npcObjectPointer.transform.SetParent(this.transform);
        _NamingFreshNpc(_npcObjectPointer);
        Debug.Log("SPAWNED SHIP: " + _npcObjectPointer.name + " @ " + _npcObjectPointer.transform.position.ToString());

        // TODO: MAYBE A KILL NPC FUNCTION THAT DOES => DESTROY(NPC) && npcSpawned--;
        // _RegisterFreshNpc(_npcObjectPointer);
    }

    // Depends on the outcome of the coroutine, npc will be registered to the db or destroyed
    private void _RegisterFreshNpc(GameObject npc)
    {
        StartCoroutine(_gm.saveLoad.Register(npc.GetComponent<NPC>(), (retVal) =>
        {
            if(retVal == 1)
            {
                Debug.Log("SUCCESS: NPC REGISTERED");
            }
            else if(retVal == -1)
            {
                Debug.Log("FAIL: NPC REGISTRATION FAILED, DELETE [" + npc.name + "]");
                Destroy(npc);
            }
            else
            {
                Debug.Log("WORST POSSIBLE OUTCOME!!!");
                Destroy(npc);

            }
        }));

    }

    private void _NamingFreshNpc(GameObject newNpc) // names are given as: "NPC_" + npcSpawned
    {
        // int npcID = npcs.transform.childCount;
        int npcSpawned = PlayerPrefs.GetInt("npcSpawned", 0);
        newNpc.name = "NPC_" + npcSpawned;
        newNpc.GetComponent<NPC>().SetNPCID(npcSpawned);
        PlayerPrefs.SetInt("npcSpawned", ++npcSpawned);
    }

}
