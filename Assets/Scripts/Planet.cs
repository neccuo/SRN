using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    // size realm
    public float scaleBase = 1f;
    public float scaleRange = 1.2f;

    // movement realm
    public float speedBase = 1f;
    public float speedRange = 5f;

    // private bool isForward = true;

    public Transform sunLocation;

    Vector2 sunPos;
    public Vector2 planetPos;


    float rotationRadius; // distance between sun and planet
    float angularSpeed = 2f;

    float posX, posY;
    public float angle;


    void Start()
    {
        sunPos = sunLocation.position;
        planetPos = transform.position;

        rotationRadius = Vector2.Distance(sunPos, planetPos);
        Debug.Log(rotationRadius.ToString());

        InitScale();
        InitAngle();
    }

    // Update is called once per frame
    void Update()
    {
        posX = sunLocation.position.x + Mathf.Cos(angle) * rotationRadius;
        posY = sunLocation.position.y + Mathf.Sin(angle) * rotationRadius;
        transform.position = new Vector2(posX, posY);
        angle -= Time.deltaTime * angularSpeed;

        if (angle >= 360f)
            angle = 0f;
        else if (angle < 0f)
            angle = 360f;

        Debug.LogFormat("Angle: {0}", angle);
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

    void InitAngle()
    {
        Debug.LogFormat("sun: {0}, planet: {1}", ((Vector2) sunLocation.position).ToString(), ((Vector2) transform.position).ToString());
        angle = getAngle(transform.position, sunLocation.position); //Vector2.Angle(transform.position, sunLocation.position);
        
        /*posX = sunLocation.position.x + Mathf.Cos(angle) * rotationRadius;
        posY = sunLocation.position.y + Mathf.Sin(angle) * rotationRadius;
        Debug.LogFormat("sun: {0}, planet: {1}", sunLocation.position.ToString(), transform.position.ToString());*/

        Debug.LogFormat("Initial angle: {0}", angle);
        Debug.LogFormat("Initial position: {0}, {1}", posX, posY);

    }

    void InitVelocity()
    {

    }

    public float getAngle(Vector2 me, Vector2 target)
    {
        return Mathf.Atan2(target.y - me.y, target.x - me.x) * (180 / Mathf.PI);
    }
}
