using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public Item(int id, int price)
    {
        itemId = id;
        itemPrice = price;
    }
    public int itemId { get; set; }
    public int itemPrice { get; set; }

    public override string ToString()
    {
        return "id: " + itemId + " // price: " + itemPrice + " desu.";
    }
}

public class ShopStock : MonoBehaviour
{
    public List<Item> itemStock;
    public int stockSize = 3; // TODO: RANDOMIZE THE VALUE AND MAKE THEM VISIBLE IN THE POPUP

    public void Awake()
    {
        itemStock = new List<Item>();
        Item tempItem;
        int tempNum;
        for(int i = 0; i < stockSize; i++)
        {
            tempNum = Random.Range(1000, 10000);
            tempItem = new Item(i, tempNum);
            itemStock.Add(tempItem);
        }
    }

    public void PrintStock()
    {
        Debug.Log("--" + gameObject.name + "'s item stock!--");
        foreach(Item item in itemStock)
        {
            Debug.Log(item.ToString());
        }
    }
}
