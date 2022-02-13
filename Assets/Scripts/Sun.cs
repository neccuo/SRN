using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    // float baseScale = 2.6f;
    float scaleRange = 1f;

    void Start()
    {
        Vector3 temp;
        float randNum = Random.Range(0, scaleRange);
        temp = transform.localScale;
        temp.x += randNum;
        temp.y += randNum;

        transform.localScale = temp;
    }

}
