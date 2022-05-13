using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
    public GameObject npcPrefab;
    public GameObject planets;
    public GameObject npcs;

    public static int npcSpawned = 0;

    private GameObject _npcObjectPointer;

    private Time time;

    private float _timer;

    private GameManager _gm;

    // it receives a tick and works

    void Start()
    {
        _gm = GameManager.Instance;
        _timer = 0.0f;
        if(npcPrefab.tag != "NPC")
        {
            Debug.LogError("NPC tag is not NPC, blueprint is flawed.");
        }
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > 1.5f && planets.transform.childCount > 0)
        {
            // DefineFreshNpc();
            _timer = 0.0f;
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            DefineFreshNpc();
        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            StartCoroutine(_gm.saveLoad.LoadDB());
        }
    }

    public Vector3 PickRandomPlanetPos()
    {
        int childCount = planets.transform.childCount;
        if(childCount == 0)
        {
            Debug.LogError("Planet count is 0, can't pick a planet");
            return new Vector3(0,0,-100);
        }
        int rndNum = Random.Range(0, childCount);
        Transform chosenPlanet = planets.transform.GetChild(rndNum);
        Debug.Log(chosenPlanet.name + " is chosen. Will spawn in: " + chosenPlanet.position);
        return chosenPlanet.position;
    }

    public void LoadExistingNpc(int id, string name, float x_axis, float y_axis)
    {
        // let's say a tick just came and you have to set the attributes of the npc

        // POINTER ASSIGNMENT REALM
        _npcObjectPointer = npcPrefab;

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

        // SPACESHIP SPRITE RENDERER REALM
        SpriteRenderer spriteRendererRef = _npcObjectPointer.GetComponentInChildren<SpriteRenderer>();
        spriteRendererRef.sprite = Resources.Load<Sprite>("Sprites/Ships/spaceship-2");

        // SPACESHIP SCRIPT REALM
        Spaceship spaceship = _npcObjectPointer.GetComponentInChildren<Spaceship>();
        spaceship.maxHealth = 100;
        spaceship.currentHealth = 100;
        spaceship.speed = 25;

        _npcObjectPointer = Instantiate(_npcObjectPointer);
        _npcObjectPointer.transform.SetParent(npcs.transform);
        _npcObjectPointer.name = name;

        Debug.Log("SPAWNED SHIP: " + _npcObjectPointer.name + " @ " + _npcObjectPointer.transform.position.ToString());

    }

    public void DefineFreshNpc()
    {
        // let's say a tick just came and you have to set the attributes of the npc

        // POINTER ASSIGNMENT REALM
        _npcObjectPointer = npcPrefab;

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

        // SPACESHIP SPRITE RENDERER REALM
        SpriteRenderer spriteRendererRef = _npcObjectPointer.GetComponentInChildren<SpriteRenderer>();
        spriteRendererRef.sprite = Resources.Load<Sprite>("Sprites/Ships/spaceship-2");

        // SPACESHIP SCRIPT REALM
        Spaceship spaceship = _npcObjectPointer.GetComponentInChildren<Spaceship>();
        spaceship.maxHealth = 100;
        spaceship.currentHealth = 100;
        spaceship.speed = 25;


        _npcObjectPointer = Instantiate(_npcObjectPointer);
        _npcObjectPointer.transform.SetParent(npcs.transform);
        _NamingFreshNpc(_npcObjectPointer);
        Debug.Log("SPAWNED SHIP: " + _npcObjectPointer.name + " @ " + _npcObjectPointer.transform.position.ToString());

        // TODO: MAYBE A KILL NPC FUNCTION THAT DOES => DESTROY(NPC) && npcSpawned--;
        _RegisterFreshNpc(_npcObjectPointer);
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
