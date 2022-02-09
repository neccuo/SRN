using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public float baseScale = 4f;
    public float scaleRange = 5f;

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
