using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickPoint : MonoBehaviour
{
    public Vector2 origin;
    public Vector2 target;


    public GameObject crossObject;
    public float crossScale = 5f;

    public GameObject followObject;

    public GameObject dotObject;
    public float dotScale = 1f;

    List<Object> dotPointers;
    Object crossPointer;
    Object followPointer;


    float targetAngle;

    void Start()
    {
        dotPointers = new List<Object>();
        // origin = transform.position;
        crossObject.transform.localScale = new Vector2(crossScale, crossScale);
        followObject.transform.localScale = new Vector2(crossScale, crossScale); // BEWARE, SAME AS crossScale
        dotObject.transform.localScale = new Vector2(dotScale, dotScale);
    }

    public void SpawnCrossArrow() // Spawn cross with dots
    {
        SpawnTip("Cross");
        SpawnDots();
    }

    public void SpawnFollowArrow() // spawn follow with dots
    {
        SpawnTip("Follow");
        SpawnDots();
    }

    public void DestroyArrow()
    {
        // I don't know if we are using cross or follow at the time we are deleting it.
        DestroyCross();
        DestroyFollow();
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

    void DestroyFollow()
    {
        Destroy(followPointer);
    }

    private void SpawnTip(string order) // spawn only the cursor, the pointer (whether it is cross or follow)
    {
        DestroyArrow();
        target = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(origin);
        targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        switch(order)
        {
            case "Cross":
            {
                crossPointer = Instantiate(crossObject, target, Quaternion.AngleAxis(targetAngle - 90, Vector3.forward));
                break;
            }
            case "Follow":
            {
                followPointer = Instantiate(followObject, target, Quaternion.AngleAxis(targetAngle - 45, Vector3.forward));
                break;
            }
            default:
            {
                throw new MissingComponentException("" + order + "is not an available cursor tip.");
            }
        }
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

}
