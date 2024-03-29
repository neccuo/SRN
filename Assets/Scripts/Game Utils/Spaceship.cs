using System;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    private Dictionary<int, (float, float)> engineLocationsByHullId;
    [SerializeField] private GameObject engineBeam;

    // stats realm
    public float maxHealth = 100;
    public float currentHealth;
    public float speed = 10f;

    [SerializeField]
    private int _hullID = -1;

    // Rotation realm (work in progress)
    public float rotationBounds = 30; // 150 - 180 - 210
    public float rotationSpeed = 10f;
    private readonly float initialRotationY = 180f;
    private int rotationUpOrDown = 1; // 1 up, -1 down

    // Sprite realm
    // public Sprite sprite;
    private SpriteRenderer _spriteRenderer;

    // place it to somewhere more appropriate
    void InitEngineLocationHash()
    {
        engineLocationsByHullId = new Dictionary<int, (float, float)>();
        engineLocationsByHullId.Add(1, (0.5f, 0.305f));
        engineLocationsByHullId.Add(2, (0.5f, 0.19f));
        engineLocationsByHullId.Add(3, (0.5f, 0.3f));
        engineLocationsByHullId.Add(4, (0.5f, 0.32f));
        engineLocationsByHullId.Add(5, (0.5f, 0.137f));
        engineLocationsByHullId.Add(6, (0.5f, 0.205f));
        engineLocationsByHullId.Add(7, (0.5f, 0.125f));
    }

    float GetScaleFromMaxHealth()
    {
        if(maxHealth <= 100) { return 0.5f; }
        else if(maxHealth >= 1000) { return 5f; }
        else { return maxHealth / 200; }
    }


    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        InitEngineLocationHash();

        // ************* DON'T FORGET ***********
        engineBeam.SetActive(false); 
        
        //float shipScale = GetScaleFromMaxHealth();
        //transform.localScale = new Vector3(shipScale, shipScale, 1);

        //spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        //spriteRenderer.sprite = sprite;

        //transform.rotation.Set(transform.rotation.x, initialRotationY, transform.rotation.z, 0);
        currentHealth = maxHealth;

    }

    void Update()
    {
        // RotationAnimation(transform.rotation.y);
        /*if(Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }*/
    }

    public void SetHullID(int id)
    {
        _hullID = id;
        // hull id ile bir şeyler yapacaksın, engine location vsvs
    }

    public int GetHullID()
    {
        return _hullID;
    }

    public void ChangeHull(Sprite newSprite)
    {
        _spriteRenderer.sprite = newSprite;
    }

    public void TakeDamage(float dmg)
    {
        if (currentHealth <= 0) { return; }
        float takenDamage;

        if(currentHealth - dmg < 0) // overkill
        {
            takenDamage = currentHealth;
            currentHealth = 0;
        }
        else // normal damage
        {
            takenDamage = dmg;
            currentHealth -= dmg;
        }
        Debug.Log("Took " + takenDamage + " damage");
    }

    public void RepairDamage(float repair)
    {
        if(currentHealth == maxHealth) { return; }
        float repairedAmount;

        if(currentHealth + repair > maxHealth)  // overheal
        {
            repairedAmount = maxHealth - currentHealth;
            currentHealth = maxHealth;
        }
        else // normal repair
        {
            repairedAmount = repair;
            currentHealth += repair; 
        }
        Debug.Log("Repaired " + (repairedAmount) + " health");
    }

    void RotationAnimation(float currentRotationY)
    {
        if(currentRotationY >= initialRotationY + rotationBounds) //  current >= 210
        {
            rotationUpOrDown = -1; // go down
        }
        else if(currentRotationY <= initialRotationY - rotationBounds) // current <= 150
        {
            rotationUpOrDown = 1; // go up
        }
        float newRotationY = currentRotationY + (rotationUpOrDown * rotationSpeed * Time.deltaTime); // currentRotation +/- rotationSpeed*delta
        Debug.Log(newRotationY);
        transform.rotation = new Quaternion(0, newRotationY, transform.rotation.z, 0);
    }
}
