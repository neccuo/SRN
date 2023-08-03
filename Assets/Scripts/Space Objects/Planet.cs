using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{

    #region size realm
    // float scaleBase;
    float scaleRange;

    Controller controller;

    #endregion

    #region movement realm
    float speedBase;
    float speedRange;

    [SerializeField] private PlanetManager planetManager;
    [SerializeField] private int planetID;
    [SerializeField] private float angularSpeed;
    [SerializeField] private int systemID;
    [SerializeField] private int spriteID;
    [SerializeField] private int shopID;

    #endregion

    private Vector2 _sunLocation;

    void Start()
    {
        planetManager = transform.GetComponentInParent<PlanetManager>();
        
        if(_sunLocation == null)
        {
            _sunLocation = new Vector2(0, 0);
        }
    }

    public int GetPlanetID()
    {
        return planetID;
    }

    public int GetShopID()
    {
        return shopID;
    }

    public Planet SetPlanet(int id, string name, float x, float y, float scale, float angularSpeed, int system_id, int sprite_id, int shop_id)
    {
        this.planetID = id;
        this.gameObject.name = name;
        this.transform.position = new Vector3(x, y, 0);
        this.transform.localScale = new Vector3(scale, scale, 1);
        this.angularSpeed = angularSpeed;
        this.systemID = system_id;
        this.spriteID = sprite_id;
        this.shopID = shop_id;

        return this;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        GameObject collidedObject = col.gameObject;
        // Debug.Log("" + collidedObject.name + " collided with " + this.name);
        if(collidedObject.tag == "Player")
        {
            Debug.Log("Player collided with planet: " + gameObject.name);
            // if(planetManager)
            //     planetManager.PrintPlanetStockById(this.shopID);
        }
    }

    void InitScale() // inits scale with random numbers (using designated range)
    {
        Vector3 temp;
        float randNum = Random.Range(0, scaleRange);
        temp = transform.localScale;
        temp.x += randNum;
        temp.y += randNum;

        transform.localScale = temp;
    }

    void InitAngularSpeed()
    {
        if(angularSpeed != 0f)
            return;
        float randNum = Random.Range(-speedRange, speedRange);
        // in case the randNum is too low, adding base speed just in case
        if(randNum < 0)
            angularSpeed = randNum - speedBase;
        else
            angularSpeed = randNum + speedBase;
    }

    // COME AGAIN
    void OneDayOfOrbit()
    {
        Vector2 center = new Vector2(0, 0);
        Vector2 pos = transform.position;
        Quaternion rot = Quaternion.AngleAxis(angularSpeed, new Vector3(0, 0, 1));
        Vector2 dir = pos - center;
        dir = rot * dir;
        transform.position = center + dir;
    }


    // taken from transform.RotateAround()
    public void OrbitSun(Vector2 center, float angle)
    {
        Vector2 pos = transform.position;
        Quaternion rot = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
        Vector2 dir = pos - center;
        dir = rot * dir;
        transform.position = center + dir;
    }

    public void OrbitSun()
    {
        Vector2 center = new Vector2(0, 0);
        float angle = angularSpeed * Time.deltaTime;
        Vector2 pos = transform.position;
        Quaternion rot = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
        Vector2 dir = pos - center;
        dir = rot * dir;
        transform.position = center + dir;
    }
}
