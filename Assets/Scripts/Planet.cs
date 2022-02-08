using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    // size realm
    // float scaleBase;
    float scaleRange;

    // movement realm
    float speedBase;
    float speedRange;
    public float angularSpeed;

    public Transform sunLocation;

    void Start()
    {
        //scaleBase = 1f;
        scaleRange = 1.2f;
        speedBase = 10f;
        speedRange = 50f;
        angularSpeed = 0f;

        InitScale();
        InitAngularSpeed();
    }

    void Update()
    {
        OrbitSun(sunLocation.position, angularSpeed * Time.deltaTime);

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
