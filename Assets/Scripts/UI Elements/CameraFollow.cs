using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;

    private float _zOffSet;

    void Start()
    {
        _zOffSet = transform.position.z; // originally assigned value (-10)
    }

    void LateUpdate()
    {
        transform.position = target.position + new Vector3(0, 0, _zOffSet);
    }
}
