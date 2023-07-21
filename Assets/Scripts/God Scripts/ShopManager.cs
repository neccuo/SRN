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

    // UI REALM
    [SerializeField] public GameObject ShopBarObject;

    // It has ItemSlot items in it.
    [SerializeField] public GameObject ItemsObject;

    [SerializeField] public Image highlightedItemImageUI;

    // The Image inside the ItemSlot object
    [SerializeField] private GameObject _shopItemPrefab;


    [SerializeField] private SystemDB _systemDB;

    private List<ShopItem> _shopItems;

    GameObject highlightedItem;
    
    [NonSerialized]
    public GameState shopState = GameState.ShopState;

    private int _currentShopID = -1;

    string texturePath = "Sprites/Item_Sprites/itemsprites-items";
    Sprite[] sprites;

    public ShopManager()
    {
        shopManager = this;
    }

    private void Start() 
    {
        // A JOB FOR ITEM MANAGER, KEEPING IT HERE TEMPORARILY
        sprites = Resources.LoadAll<Sprite>(texturePath);
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
        highlightedItemImageUI.enabled = true;
        highlightedItemImageUI.sprite = box.GetComponent<Image>().sprite;
    }
    
    public void OpenByShopID(int id)
    {
        _currentShopID = id;
        _shopItems = _systemDB.GetShopItemsById(id);
        OpenPopup();
    }

    private void OpenPopup()
    {
        OnShopBarEnable();
        ShopBarObject.SetActive(true);
    }

    // Attached to the close button
    private void ClosePopup()
    {
        OnShopBarDisable();
        ShopBarObject.SetActive(false);
    }

    // StartUp()
    private void OnShopBarEnable()
    {
        Debug.Log("Shop Bar is Enabled");

        int slotIndex = 0;
        foreach(ShopItem item in _shopItems)
        {
            int itemQt = item.quantity;
            for(int i = 0; i < itemQt; ++i)
            {
                SetShopItem(slotIndex, item.id);
                slotIndex++;
            }

        }
        // TODO: CHANGE STATES ACCORDINGLY IF IT IS ENABLED AT THE START OF THE GAME
        if(GameManager.Instance.GetCurrentState() != shopState)
        {
            Controller.ControllerGod.ChangeState(shopState);
        }
    }

    // OnClose()
    private void OnShopBarDisable()
    {
        Debug.Log("Shop Bar is Disabled");

        NullifyHighlightedItem();
        Transform itemsTrans = ItemsObject.transform;

        foreach(Transform itemSlot in itemsTrans)
        {
            // ASSUMING itemSlot HAS ONE CHILD3
            DestroyAllChildren(itemSlot);

            Button itemSlotButton = itemSlot.gameObject.GetComponent<Button>();
            itemSlotButton.interactable = false;
            itemSlotButton.onClick.RemoveAllListeners();

        }

        Controller.ControllerGod.ChangeState(GameState.PlanMovement);
    }

    private void SetShopItem(int slotIndex, int itemId)
    {
        GameObject newShopItem = Instantiate(_shopItemPrefab);
        Image itemImage = newShopItem.GetComponent<Image>();

        Transform childItemSlot = ItemsObject.transform.GetChild(slotIndex);
        Button myButton = childItemSlot.gameObject.GetComponent<Button>();

        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(() => SetHighlightedItem(newShopItem));
        myButton.interactable = true;

        // DEFINITELY CHANGE, PLEASE
        string spriteName = "";
        if(itemId == 1)
            spriteName = "drone_0";
        else if(itemId == 2)
            spriteName = "radar_1";
        else if(itemId == 3)
            spriteName = "engine_3";

        Sprite mySprite = System.Array.Find(sprites, s=> s.name == spriteName);
        itemImage.sprite = mySprite;


        // NOT SURE ABOUT ,false though...
        newShopItem.transform.SetParent(childItemSlot, false);
    }

    private void DestroyAllChildren(Transform trans)
    {
        int childCount = trans.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = trans.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    private void NullifyHighlightedItem()
    {
        highlightedItem = null;
        highlightedItemImageUI.enabled = false;
        highlightedItemImageUI.sprite = null;
    }

}
