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
    public float angularSpeed;

    #endregion

    public Transform sunLocation;

    void Start()
    {
        //scaleBase = 1f;
        scaleRange = 0.6f;
        speedBase = 5f;
        speedRange = 20f;
        angularSpeed = 0f;

        if(sunLocation == null)
        {
            sunLocation = GameObject.Find("Sun").transform;
        }

        InitScale();
        InitAngularSpeed();
    }

    void Update()
    {
        OrbitSun(sunLocation.position, angularSpeed * Time.deltaTime);
    }

    /*void OnMouseDown() // TO BE CONTINUED
    {
        Debug.Log("You just clicked " + this.name);
    }*/

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

        // Debug.LogFormat("angular speed: {0}", angularSpeed);

    }


    // taken from transform.RotateAround()
    void OrbitSun(Vector2 center, float angle)
    {
        Vector2 pos = transform.position;
        Quaternion rot = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
        Vector2 dir = pos - center;
        dir = rot * dir;
        transform.position = center + dir;
    }
}
