using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShipSpawner : MonoBehaviour
{
    public GameObject npcPrefab;
    public GameObject planets;
    public GameObject npcs;

    public static int npcSpawned = 0;

    private GameObject _npcObjectPointer;

    private Time time;

    private float _timer;

    // it receives a tick and works

    void Awake()
    {
        // TODO: GET EVERY NPC OUT OF THE DB, ASSIGN THE VALUES
        // StartCoroutine(Load());
    }

    void Start()
    {
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
            StartCoroutine(Load());
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

        // SPACESHIP SPRITE RENDERER REALM
        SpriteRenderer spriteRendererRef = _npcObjectPointer.GetComponentInChildren<SpriteRenderer>();
        spriteRendererRef.sprite = Resources.Load<Sprite>("Sprites/Ships/spaceship-2");

        // Debug.Log(spriteRendererRef.sprite);
        
        // SPACESHIP SCRIPT REALM

        Spaceship spaceship = _npcObjectPointer.GetComponentInChildren<Spaceship>();
        spaceship.maxHealth = 100;
        spaceship.currentHealth = 100;
        spaceship.speed = 25;


        _npcObjectPointer = Instantiate(_npcObjectPointer);
        _npcObjectPointer.transform.SetParent(npcs.transform);
        _NamingNpcs(_npcObjectPointer);

        StartCoroutine(Register(_npcObjectPointer.GetComponent<NPC>()));
        // npcSpawned++;
        Debug.Log("SPAWNED SHIP: " + _npcObjectPointer.name + " @ " + _npcObjectPointer.transform.position.ToString());
    }

    private void _NamingNpcs(GameObject newNpc) // names are given as: "NPC_" + npcSpawned
    {
        // int npcID = npcs.transform.childCount;
        int npcSpawned = PlayerPrefs.GetInt("npcSpawned", 0);
        newNpc.name = "NPC_" + npcSpawned;
        newNpc.GetComponent<NPC>().SetNPCID(npcSpawned);
        PlayerPrefs.SetInt("npcSpawned", ++npcSpawned);
    }

    IEnumerator Load()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/sqlconnect/load.php");

        yield return www.SendWebRequest();
        
        if( www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("load: success");
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("load: fail");
            Debug.Log(www.error);
            Debug.Log(www.result);

        }

    }

    IEnumerator Register(NPC newNpc)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", newNpc.GetNPCID().ToString());
        form.AddField("name", newNpc.gameObject.name);
        form.AddField("credits", "1500");
        form.AddField("race", "human");
        form.AddField("hull_id", "1");
        form.AddField("x_axis", newNpc.transform.position.x.ToString());
        form.AddField("y_axis", newNpc.transform.position.y.ToString());
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/register.php", form);
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("register: success");
        }
        else
        {
            Debug.Log("register: fail");
            Debug.Log(www.error);
            Debug.Log(www.result);
        }
    }
}
