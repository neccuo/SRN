using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera cam;
    public Transform subject;
    public float parallaxFactor;


    Vector2 startPosition;
    float startZ;

    Vector2 travel => (Vector2)cam.transform.position - startPosition;

    void Start()
    {
        CheckParallax();
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    void Update()
    {
        transform.position = startPosition + travel * parallaxFactor;
    }

    void CheckParallax()
    {
        if (parallaxFactor > 1)
            parallaxFactor = 1;
        else if (parallaxFactor < -1)
            parallaxFactor = -1;
    }
    
}
