using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickPoint : MonoBehaviour
{
    public Vector2 origin;
    public Vector2 target;


    public GameObject crossObject;
    public float crossScale = 5f;

    public GameObject dotObject;
    public float dotScale = 1f;

    List<Object> dotPointers;
    Object crossPointer;

    // Start is called before the first frame update
    void Start()
    {
        dotPointers = new List<Object>();
        origin = transform.position;

        crossObject.transform.localScale = new Vector2(crossScale, crossScale);
        dotObject.transform.localScale = new Vector2(dotScale, dotScale);

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnCursor();
            SpawnDots();
        }
        
    }

    void SpawnDots()
    {
        if(dotPointers.Count > 0)
        {
            for(int i = 0; i < dotPointers.Count; i++)
            {
                Destroy(dotPointers[i]);
            }
        }
        
        Vector2 difference = target - origin;
        Vector2 unit = difference.normalized;


        for(Vector2 parser = origin; parser.magnitude < target.magnitude; parser += unit)
        {
            dotPointers.Add(Instantiate(dotObject, parser, new Quaternion(0, 0, 0, 0)));
        }
    }

    void SpawnCursor()
    {
        Destroy(crossPointer);
        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        crossPointer = Instantiate(crossObject, target, Quaternion.AngleAxis(angle - 90, Vector3.forward));
        Debug.Log("" + crossPointer.ToString() + " is created");

    }
}
