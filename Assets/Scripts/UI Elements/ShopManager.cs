using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem
{
    public int id { get; set; }
    public string name { get; set; }
    public decimal price { get; set; }
    public int quantity { get; set; }
}

public class ShopManager : MonoBehaviour
{
    public static ShopManager shopManager;

    [SerializeField] public GameObject ShopBarObject;
    [SerializeField] public Image highlightedItemSlotUI;

    [SerializeField] private SystemDB _systemDB;

    List<ShopItem> shopItems = new List<ShopItem>();

    GameObject highlightedItem;
    
    [NonSerialized]
    public GameState shopState = GameState.ShopState;

    public ShopManager()
    {
        shopManager = this;
    }

    /* MORE BADASS VERSION OF AWAKE, IT RUNS FIRST
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    void OnBeforeSceneLoadRuntimeMethod()
    {
        Awake();
    }*/

    public void SetHighlightedItem(GameObject box)
    {
        highlightedItem = box;
        highlightedItemSlotUI.sprite = box.GetComponent<Image>().sprite;
    }
    
    public void OpenByShopID(int id)
    {
        OpenPopup();
    }

    public void OpenPopup()
    {
        ShopBarObject.SetActive(true);
        OnShopBarEnable();
    }

    // Attached to the close button
    public void ClosePopup()
    {
        ShopBarObject.SetActive(false);
        OnShopBarDisable();
    }

    void OnShopBarEnable()
    {
        Debug.Log("Shop Bar is Enabled");
        InitializationProcess();

        // TODO: CHANGE STATES ACCORDINGLY IF IT IS ENABLED AT THE START OF THE GAME
        if(GameManager.Instance.GetCurrentState() != shopState)
        {
            Controller.ControllerGod.ChangeState(shopState);
        }
    }

    void OnShopBarDisable()
    {
        Debug.Log("Shop Bar is Disabled");
        Controller.ControllerGod.ChangeState(GameState.PlanMovement);
    }

    private void InitializationProcess()
    {
        // TODO: GATHER DATA OF THE ITEMS FROM SOMEWHERE AND THEN ASSIGN THE VALUES.
        GameObject ItemsObject = ShopBarObject.transform.Find("ShopPopup").Find("Items").gameObject;
        // foreach(Transform child in ItemsObject.transform)
        // {
        //     // Debug.Log(child.gameObject.name);
        //     items.Add(child.gameObject);
        // }
        // _SetItemPrices();
    }
}
