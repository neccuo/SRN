using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickPoint : MonoBehaviour
{
    public Vector2 origin;
    public Vector2 target;

    public GameObject cross;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            SpawnCursor();
        }
        
    }

    void SpawnCursor()
    {
        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Object abc = Instantiate(cross, target, new Quaternion(0, 0, 0, 0));
        Debug.Log("" + abc.ToString() + " is created");

    }
}
