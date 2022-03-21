using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBarManager : MonoBehaviour
{
    List<GameObject> items = new List<GameObject>();
    public GameState officialState;


    void Awake()
    {
        officialState = GameState.ShopState;
    }
    void Start()
    {
        // TODO: GATHER DATA OF THE ITEMS FROM SOMEWHERE AND THEN ASSIGN THE VALUES.
        GameObject ItemsObject = transform.Find("ShopPopup").Find("Items").gameObject;
        foreach(Transform child in ItemsObject.transform)
        {
            Debug.Log(child.gameObject.name);
            items.Add(child.gameObject);
        }
        _SetItemPrices();
    }

    private void _SetItemPrices()
    {
        int rPrice;
        for(int i = 0; i < items.Count; i++)
        {
            rPrice = Random.Range(100, 1000);
            items[i].transform.Find("PriceLabel").GetComponent<Text>().text = "Cr: 00" + rPrice.ToString();
        }
    }

    void OnEnable()
    {
        // TODO: CHANGE STATES ACCORDINGLY IF IT IS ENABLED AT THE START OF THE GAME
        if(GameManager.Instance.GetCurrentState() != officialState)
        {
            Controller.ControllerGod.ChangeState(officialState);
        }
        Debug.Log("Shop Bar is Enabled");
    }

    void OnDisable()
    {
        Debug.Log("Shop Bar is Disabled");
        Controller.ControllerGod.ChangeState(GameState.PlanMovement);
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
