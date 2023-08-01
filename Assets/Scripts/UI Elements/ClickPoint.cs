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

public enum Tip
{
    Cross,
    Follow
}


public class ClickPoint : MonoBehaviour
{
    private Vector2 _origin;
    private Vector2 _target;

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
        crossObject.transform.localScale = new Vector2(crossScale, crossScale);
        followObject.transform.localScale = new Vector2(crossScale, crossScale); // BEWARE, SAME AS crossScale
        dotObject.transform.localScale = new Vector2(dotScale, dotScale);
    }

    public void SpawnArrow(Vector2 origin, Vector2 target, Tip tip)
    {
        SetOrigin(origin);
        SetTarget(target);
        SpawnTip(tip);
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

    private void SetOrigin(Vector2 v2)
    {
        _origin = v2;
    }

    private void SetTarget(Vector2 v2)
    {
        _target = v2;
    }

    private void DestroyDots()
    {
        if(dotPointers.Count > 0)
        {
            for(int i = 0; i < dotPointers.Count; i++)
            {
                Destroy(dotPointers[i]);
            }
        }
    }

    private void DestroyCross()
    {
        Destroy(_crossPointer);
    }

    private void DestroyFollow()
    {
        Destroy(_followPointer);
    }

    private void CreateText(VDT type) // 0: dist, 1: days
    {
        Vector2 newVect = Camera.main.WorldToScreenPoint(_target);
        text.transform.position = new Vector3(newVect.x, newVect.y, 0);
        Vector2 diff = _target - _origin;
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

    private int FloatCeil(float value)
    {
        return (int) (value+1);
    }

    private float FloatPrecisionSimplifier(float value, int degree)
    {
        int temp = (int) Mathf.Pow(10, degree);
        int val = (int) (value*temp);
        return  ((float) val / (float) temp);
    }

    private void KillDistText()
    {
        text.SetActive(false);
    }

    private void SpawnTip(Tip order) // spawn only the cursor, the pointer (whether it is cross or follow)
    {
        DestroyArrow();
        // target = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Vector2 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(origin);
        Vector2 dir = _target - _origin;

        targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        CreateText(VDT.Time);
        switch(order)
        {
            case Tip.Cross:
            {
                _crossPointer = Instantiate(crossObject, _target, Quaternion.AngleAxis(targetAngle - 90, Vector3.forward));
                SetChild(_crossPointer.transform);
                break;
            }
            case Tip.Follow:
            {
                _followPointer = Instantiate(followObject, _target, Quaternion.AngleAxis(targetAngle - 45, Vector3.forward));
                SetChild(_followPointer.transform);
                break;
            }
            default:
            {
                throw new MissingComponentException("" + order + "is not an available cursor tip.");
            }
        }
    }

    private void SpawnDots()
    {
        DestroyDots();
        Vector2 difference = _target - _origin;
        Vector2 unit = difference.normalized;
        int counter = 0;
        GameObject temp;
        for(Vector2 parser = _origin /*+ unit*/; (_origin - parser).magnitude < (_origin - _target).magnitude; parser += unit)
        {
            temp = Instantiate(dotObject, parser, Quaternion.AngleAxis(targetAngle - 90, Vector3.forward));
            dotPointers.Add(temp);

            SetChild(temp.transform);
            counter++;
        }
    }

    private void SetChild(Transform child) // MAKING EVERY INSTANCE A CHILD OF THIS OBJ
    {
        child.SetParent(transform); 
    }

}
