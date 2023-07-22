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
    [SerializeField] private Button _confirmBuyButton;

    // The Image inside the ItemSlot object
    [SerializeField] private GameObject _shopItemPrefab;


    [SerializeField] private SystemDB _systemDB;

    private List<ShopItem> _shopItems;

    private ShopItem _highlightedItem;
    
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

    private void Update() 
    {
        // if(ShopBarObject.activeSelf && Input.GetKeyDown(KeyCode.Z))
        // {
        //     NullifyHighlightedItem();
        //     NullifyButtons();

        //     if(_currentShopID == 1)
        //     {
        //         OpenByShopID(2);
        //     }
        //     else if(_currentShopID == 2)
        //     {
        //         OpenByShopID(1);
        //     }
        // }
    }

    /* MORE BADASS VERSION OF AWAKE, IT RUNS FIRST
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    void OnBeforeSceneLoadRuntimeMethod()
    {
        Awake();
    }*/

    private void ItemSlotClicked(GameObject box)
    {
        SetHighlightedItem(box);
        _confirmBuyButton.onClick.RemoveAllListeners();
        _confirmBuyButton.onClick.AddListener(() => BuyHighlightedItem());

        _confirmBuyButton.interactable = true;
    }

    private void BuyHighlightedItem()
    {
        _systemDB.BuyItem(_currentShopID, _highlightedItem.id);
        Debug.Log("Bought: " + _highlightedItem.ToString());

        ResetShop();
    }

    private void ResetShop()
    {
        NullifyHighlightedItem();
        NullifyButtons();

        OpenByShopID(_currentShopID);
    }

    private void CloseShop()
    {
        NullifyHighlightedItem();
        NullifyButtons();

        _currentShopID = -1;
    }

    private void SetHighlightedItem(GameObject itemObj)
    {
        // yarak gibi kod geliyor
        _highlightedItem = itemObj.GetComponent<ShopItemContainer>().shopItem;
        highlightedItemImageUI.enabled = true;
        highlightedItemImageUI.sprite = itemObj.GetComponent<Image>().sprite;
    }
    
    public void OpenByShopID(int id)
    {
        _currentShopID = id;
        _shopItems = _systemDB.GetShopItemsByShopId(id);
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
                SetShopItem(slotIndex, item);
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
        CloseShop();

        Controller.ControllerGod.ChangeState(GameState.PlanMovement);
    }


    private void SetShopItem(int slotIndex, ShopItem item)
    {
        GameObject newShopItemObj = Instantiate(_shopItemPrefab);
        Image itemImage = newShopItemObj.GetComponent<Image>();
        ShopItemContainer container = newShopItemObj.GetComponent<ShopItemContainer>();

        // TO READ ITEM INFO
        container.shopItem = item;

        // UI SHIT
        Transform childItemSlot = ItemsObject.transform.GetChild(slotIndex);
        Button myButton = childItemSlot.gameObject.GetComponent<Button>();

        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(() => ItemSlotClicked(newShopItemObj));
        myButton.interactable = true;

        // DEFINITELY CHANGE, PLEASE
        string spriteName = "";
        if(item.id == 1)
            spriteName = "drone_0";
        else if(item.id == 2)
            spriteName = "radar_1";
        else if(item.id == 3)
            spriteName = "engine_3";

        Sprite mySprite = System.Array.Find(sprites, s=> s.name == spriteName);
        itemImage.sprite = mySprite;


        // NOT SURE ABOUT 'false' THOUGH...
        newShopItemObj.transform.SetParent(childItemSlot, false);
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
        _highlightedItem = null;
        highlightedItemImageUI.enabled = false;
        highlightedItemImageUI.sprite = null;
    }

    private void NullifyButtons()
    {
        _confirmBuyButton.interactable = false;
        _confirmBuyButton.onClick.RemoveAllListeners();
        Transform itemsTrans = ItemsObject.transform;

        foreach(Transform itemSlot in itemsTrans)
        {
            DestroyAllChildren(itemSlot);

            Button itemSlotButton = itemSlot.gameObject.GetComponent<Button>();
            itemSlotButton.interactable = false;
            itemSlotButton.onClick.RemoveAllListeners();
        }
    }

}
