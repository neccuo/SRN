using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
    public GameObject npcPrefab;
    public GameObject planets;

    private GameObject _npcObjectPointer;

    private Time time;

    private float _timer;

    // it receives a tick and works

    void Start()
    {
        _timer = 0.0f;
        if(npcPrefab.tag != "NPC")
        {
            Debug.LogError("NPC tag is not NPC, blueprint is flawed.");
        }
        DefineNpc();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > 1.5f)
        {
            DefineNpc();
            _timer = 0.0f;
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

    public void DefineNpc()
    {
        // let's say a tick just came and you have to set the attributes of the npc

        // POINTER ASSIGNMENT REALM
        _npcObjectPointer = npcPrefab;
        // TRANSFORM ASSIGNMENT REALM
        Transform transformRef = _npcObjectPointer.transform; // EXPERIMENTAL

        Vector3 loc = PickRandomPlanetPos(); // ASSUMING WE ARE WORKING WITH PLANETS NOW
        transformRef.position = loc; // temp
        transformRef.localScale = new Vector3(1, 1, 1);

        // NPC SCRIPT REALM
        NPC npcRef = _npcObjectPointer.GetComponent<NPC>();
        npcRef.objective = Objective.Follow;
        npcRef.patrolRange = 10;

        // SPACESHIP SPRITE RENDERER REALM
        SpriteRenderer spriteRendererRef = _npcObjectPointer.GetComponentInChildren<SpriteRenderer>();
        spriteRendererRef.sprite = Resources.Load<Sprite>("Sprites/Ships/spaceship-2");

        // Debug.Log(spriteRendererRef.sprite);
        
        // SPACESHIP SCRIPT REALM

        Spaceship spaceship = _npcObjectPointer.GetComponentInChildren<Spaceship>();
        spaceship.maxHealth = 100;
        spaceship.currentHealth = 100;
        spaceship.speed = 3;

        Instantiate(_npcObjectPointer);
    }

    // public void GetPlanets()
}
