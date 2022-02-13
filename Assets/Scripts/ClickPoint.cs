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

    float targetAngle;

    void Start()
    {
        dotPointers = new List<Object>();
        // origin = transform.position;

        crossObject.transform.localScale = new Vector2(crossScale, crossScale);
        dotObject.transform.localScale = new Vector2(dotScale, dotScale);
    }

    // Update is called once per frame
    /*void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            SpawnArrow();
        }
    }*/

    public void SpawnArrow()
    {
        SpawnCross();
        SpawnDots();
    }

    public void DestroyArrow()
    {
        //Debug.Log("DestroyArrow");
        DestroyCross();
        DestroyDots();
    }

    void DestroyDots()
    {
        if(dotPointers.Count > 0)
        {
            for(int i = 0; i < dotPointers.Count; i++)
            {
                Destroy(dotPointers[i]);
            }
        }
    }

    void DestroyCross()
    {
        Destroy(crossPointer);
    }

    void SpawnDots()
    {
        DestroyDots();

        Vector2 difference = target - origin;
        Vector2 unit = difference.normalized;

        int counter = 0;
        for(Vector2 parser = origin /*+ unit*/; (origin - parser).magnitude < (origin - target).magnitude; parser += unit)
        {
            dotPointers.Add(Instantiate(dotObject, parser, Quaternion.AngleAxis(targetAngle - 90, Vector3.forward)));

            //Debug.Log("" + dotPointers[counter].ToString() + " is created at " + parser);
            counter++;
        }
    }

    void SpawnCross()
    {
        DestroyCross();
        target = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(origin);
        targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        crossPointer = Instantiate(crossObject, target, Quaternion.AngleAxis(targetAngle - 90, Vector3.forward));
        //Debug.Log("" + crossPointer.ToString() + " is created at " + target);

    }
}
