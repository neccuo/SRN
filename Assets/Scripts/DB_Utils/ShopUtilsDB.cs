using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class ShopUtilsDB
{
    private string shopInventoryTableName = "shop_inventories";
    private string itemsTableName = "items";


    public void ShopBuy(SqliteConnection connection, int shopId, int itemId)
    {
        // Get Quantity
        int itemStock = GetShopItemQuantity(connection, shopId, itemId);
        if(itemStock == 1)
        {
            // DELETE STOCK
            DeleteShopItem(connection, shopId, itemId);
        }
        else if(itemStock > 1)
        {
            // UPDATE STOCK
            UpdateShopItemQuantity(connection, shopId, itemId, itemStock-1);
        }
        else
        {
            // SOMETHING HORRIBLE HAPPENED
            throw new Exception($"itemStock is {itemStock}");
        }
    }

    public List<ShopItem> GetAllShopItems(SqliteConnection connection, int shopId)
    {
        List<ShopItem> shopItemList = new List<ShopItem>();

        using (var command = connection.CreateCommand())
        {
            command.CommandText =
                $"SELECT items.id, items.name, ROUND(items.price * {shopInventoryTableName}.price_coefficient), {shopInventoryTableName}.quantity, items.stackable " +
                $"FROM items " +
                $"JOIN {shopInventoryTableName} ON {shopInventoryTableName}.item_id = items.id " +
                $"WHERE {shopInventoryTableName}.shop_id = {shopId} " + 
                $"ORDER BY items.id ASC;";

            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    ShopItem item = new ShopItem();
                    item.id = reader.GetInt32(0);
                    item.name = reader.GetString(1);
                    item.price = reader.GetDecimal(2);
                    item.quantity = reader.GetInt32(3);
                    item.stackable = reader.GetBoolean(4);
                    shopItemList.Add(item);
                }
            }
        }

        return shopItemList;
    }

    public int GetPrice(SqliteConnection connection, int shopId, int itemId)
    {
        int price = -1;
        using (var command = connection.CreateCommand())
        {
            string items = itemsTableName;
            string shopInv = shopInventoryTableName;
            command.CommandText = 
                $"SELECT {items}.price * {shopInv}.price_coefficient " + 
                $"AS calculated_price " +
                $"FROM {items}, {shopInv} " + 
                $"WHERE {items}.id = @ItemId " +
                $"AND {shopInv}.shop_id = @ShopId " + 
                $"AND {shopInv}.item_id = @ItemId;";

                command.Parameters.AddWithValue("@ShopId", shopId);
                command.Parameters.AddWithValue("@ItemId", itemId);

            using (IDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    double calculatedPrice = reader.GetDouble(reader.GetOrdinal("calculated_price"));
                    price = (int)calculatedPrice;
                }
                else
                    throw new Exception($"READ UNSUCCESSFUL (shopId: {shopId}, itemId: {itemId}) -> price: {price}");
            }
        }
        return price;
    }
 

    private int GetShopItemQuantity(SqliteConnection connection, int shopId, int itemId)
    {
        int itemStock = -1;
        using (var command = connection.CreateCommand())
        {
            command.CommandText = 
                $"SELECT quantity " + 
                $"FROM {shopInventoryTableName} " + 
                $"WHERE shop_id = {shopId} " + 
                $"AND item_id = {itemId};";

            using (IDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                    itemStock = reader.GetInt32(0);
                else
                    throw new Exception($"READ UNSUCCESSFUL (shopId: {shopId}, itemId: {itemId})");
            }
        }
        return itemStock;
    }

    private void DeleteShopItem(SqliteConnection connection, int shopId, int itemId)
    {
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = 
                $"DELETE FROM {shopInventoryTableName} " + 
                $"WHERE shop_id = @ShopId " + 
                $"AND item_id = @ItemId";

                // Add parameters to the command to avoid SQL injection and improve query performance
                command.Parameters.AddWithValue("@ShopId", shopId);
                command.Parameters.AddWithValue("@ItemId", itemId);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                    throw new Exception($"Shop item with ShopId: {shopId}, ItemId: {itemId} not found. Nothing was deleted.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting shop item: {ex.Message}");
        }
    }

    private void UpdateShopItemQuantity(SqliteConnection connection, int shopId, int itemId, int quantity)
    {
        try
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = 
                $"UPDATE {shopInventoryTableName} " +
                $"SET quantity = @Quantity " + 
                $"WHERE shop_id = @ShopId " + 
                $"AND item_id = @ItemId";

                // Add parameters to the command to avoid SQL injection and improve query performance
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@ShopId", shopId);
                command.Parameters.AddWithValue("@ItemId", itemId);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                    throw new Exception($"Shop item with ShopId: {shopId}, ItemId: {itemId} failed to get updated.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating shop item: {ex.Message}");
        }
    }
}
