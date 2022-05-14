using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBarManager : MonoBehaviour
{
    public static ShopBarManager ShopManager;

    List<GameObject> items = new List<GameObject>();
    
    [NonSerialized]
    public GameState officialState;

    ShopStock currentShopStockPointer;

    public ShopBarManager()
    {
        ShopManager = this;
    }

    public void SetCurrentShopStockPointer(ShopStock stock)
    {
        currentShopStockPointer = stock;
    }

    public ShopStock GetCurrentShopStockPointer()
    {
        return currentShopStockPointer;
    }

    /* MORE BADASS VERSION OF AWAKE, IT RUNS FIRST
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    void OnBeforeSceneLoadRuntimeMethod()
    {
        Awake();
    }*/
    
    /*private void Awake()
    {
        Debug.Log("PİÇ");
        ShopManager = this;
        officialState = GameState.ShopState;
    }*/

    void Start()
    {
        officialState = GameState.ShopState;
    }


    private void _SetItemPrices()
    {
        int i = 0;
        int itemPrice;
        foreach(Item item in currentShopStockPointer.itemStock)
        {
            itemPrice = item.itemPrice;
            items[i].transform.Find("PriceLabel").GetComponent<Text>().text = "Cr: " + itemPrice.ToString();
            i++;
        }
    }

    /*private void _SetItemPrices()
    {
        int rPrice;
        for(int i = 0; i < items.Count; i++)
        {
            rPrice = Random.Range(100, 1000);
            items[i].transform.Find("PriceLabel").GetComponent<Text>().text = "Cr: 00" + rPrice.ToString();
        }
    }*/

    void OnEnable()
    {
        Debug.Log("Shop Bar is Enabled");
        InitializationProcess();
        // TODO: CHANGE STATES ACCORDINGLY IF IT IS ENABLED AT THE START OF THE GAME
        if(GameManager.Instance.GetCurrentState() != officialState)
        {
            Controller.ControllerGod.ChangeState(officialState);
        }
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

    private void InitializationProcess()
    {
        // TODO: GATHER DATA OF THE ITEMS FROM SOMEWHERE AND THEN ASSIGN THE VALUES.
        GameObject ItemsObject = transform.Find("ShopPopup").Find("Items").gameObject;
        foreach(Transform child in ItemsObject.transform)
        {
            // Debug.Log(child.gameObject.name);
            items.Add(child.gameObject);
        }
        _SetItemPrices();
    }
}
