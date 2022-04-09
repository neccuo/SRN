using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum VDT // speed, distance, time
{
    Speed,
    Distance,
    Time
}

enum Tip
{
    Cross,
    Follow
}


public class ClickPoint : MonoBehaviour
{
    public Vector2 origin;
    public Vector2 target;

    public Player player;


    public GameObject crossObject;
    public float crossScale = 5f;

    public GameObject followObject;

    public GameObject dotObject;
    public float dotScale = 1f;

    public GameObject text;

    List<Object> dotPointers;
    private GameObject _crossPointer;
    private GameObject _followPointer;


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
        SpawnTip(Tip.Cross);
        SpawnDots();
    }

    public void SpawnFollowArrow() // spawn follow with dots
    {
        SpawnTip(Tip.Follow);
        SpawnDots();
    }

    public void DestroyArrow()
    {
        // I don't know if we are using cross or follow at the time we are deleting it.
        KillDistText();
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
        Destroy(_crossPointer);
    }

    void DestroyFollow()
    {
        Destroy(_followPointer);
    }

    void CreateText(VDT type) // 0: dist, 1: days
    {
        Vector2 newVect = Camera.main.WorldToScreenPoint(target);
        text.transform.position = new Vector3(newVect.x, newVect.y, 0);
        Vector2 diff = target - origin;
        string txtOut = "";
        switch(type)
        {
            case VDT.Distance:
                float cleanDist = FloatPrecisionSimplifier(diff.magnitude, 2);
                txtOut += cleanDist + " NCO";
                break;
            case VDT.Time:
                int days = FloatCeil(diff.magnitude/player.GetShipSpeed());
                txtOut += days;
                break;
            default:
                Debug.LogError("Bad input");
                break;
        }
        text.GetComponent<Text>().text = txtOut;
        text.SetActive(true);
    }

    int FloatCeil(float value)
    {
        return (int) (value+1);
    }

    float FloatPrecisionSimplifier(float value, int degree)
    {
        int temp = (int) Mathf.Pow(10, degree);
        int val = (int) (value*temp);
        return  ((float) val / (float) temp);
    }

    void KillDistText()
    {
        text.SetActive(false);
    }

    private void SpawnTip(Tip order) // spawn only the cursor, the pointer (whether it is cross or follow)
    {
        DestroyArrow();
        target = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(origin);
        targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        CreateText(VDT.Time);
        switch(order)
        {
            case Tip.Cross:
            {
                _crossPointer = Instantiate(crossObject, target, Quaternion.AngleAxis(targetAngle - 90, Vector3.forward));
                SetChild(_crossPointer.transform);
                break;
            }
            case Tip.Follow:
            {
                _followPointer = Instantiate(followObject, target, Quaternion.AngleAxis(targetAngle - 45, Vector3.forward));
                SetChild(_followPointer.transform);
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
        GameObject temp;
        for(Vector2 parser = origin /*+ unit*/; (origin - parser).magnitude < (origin - target).magnitude; parser += unit)
        {
            temp = Instantiate(dotObject, parser, Quaternion.AngleAxis(targetAngle - 90, Vector3.forward));
            dotPointers.Add(temp);

            SetChild(temp.transform);
            counter++;
        }
    }

    void SetChild(Transform child) // MAKING EVERY INSTANCE A CHILD OF THIS OBJ
    {
        child.SetParent(transform); 
    }

}
