using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class PilotTEMP
{
    public int id { get; set; }
    public string name { get; set; }
    public int credits { get; set; }
    public string race { get; set; }
    public int hull_id { get; set; }
    public float x_axis { get; set; }
    public float y_axis { get; set; }
    public float angle { get; set; }


    public int system_id { get; set; }
    
}

public class PilotUtilsDB
{
    private string pilotInventoryTableName = "pilot_inventories";
    private string pilotsTableName = "pilots";

    public List<PilotTEMP> GetAllNPCs(SqliteConnection connection)
    {
        List<PilotTEMP> pilotList = new List<PilotTEMP>();

        using (var command = connection.CreateCommand())
        {
            string pilots = pilotsTableName;

            // Not including pilot with id 0 (player)
            command.CommandText = $"SELECT * FROM {pilots} WHERE id != 0;";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    PilotTEMP temp = new PilotTEMP
                    {
                        id = OTI(reader["id"]),
                        name = reader["name"].ToString(),
                        credits = OTI(reader["credits"]),
                        race = reader["race"].ToString(),
                        hull_id = OTI(reader["hull_id"]),
                        x_axis = OTF(reader["x_axis"]),
                        y_axis = OTF(reader["y_axis"]),
                        angle = OTF(reader["angle"]),
                        system_id = OTI(reader["system_id"])
                    };
                    pilotList.Add(temp);
                }
            }
        }
        return pilotList;
    }

    public int GetPilotCredit(SqliteConnection connection, int pilotId)
    {
        int pilotCredit = -1;

        using (var command = connection.CreateCommand())
        {
            string pilots = pilotsTableName;
            command.CommandText =
                $"SELECT credits " +
                $"FROM {pilots} " +
                $"WHERE id = @PilotId;" + 

            command.Parameters.AddWithValue("@PilotId", pilotId);
            using (IDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                    pilotCredit = reader.GetInt32(0);
                else
                    throw new Exception($"READ UNSUCCESSFUL (pilotId: {pilotId})");
            }
        }
        if(pilotCredit < 0)
            throw new Exception($"ERROR: (pilotId: {pilotId}, credit: {pilotCredit})");

        return pilotCredit;
    }

    public void AddToInventory(SqliteConnection connection, int pilotId, int itemId)
    {
        int itemQt = GetInventoryItemQuantity(connection, pilotId, itemId);

        if(itemQt == 0)
        {
            InsertInventoryItem(connection, pilotId, itemId, 1);
        }
        else if(itemQt > 0)
        {
            UpdateInventoryItemQuantity(connection, pilotId, itemId, itemQt+1);
        }
        else
        {
            throw new Exception($"AddToInventory (pilotId: {pilotId}, itemId: {itemId}, itemQt: {itemQt}): Something is wrong");
        }
    }

    public void SetPilotCredits(SqliteConnection connection, int pilotId, int credits)
    {
        if(credits < 0)
        {
            throw new Exception($"Minus credits is not possible (credits: {credits})");
        }
        try
        {
            using (var command = connection.CreateCommand())
            {
                string pilots = pilotsTableName;

                command.CommandText = 
                $"UPDATE {pilots} " +
                $"SET credits = @Credits " + 
                $"WHERE id = @PilotId;";

                command.Parameters.AddWithValue("@Credits", credits);
                command.Parameters.AddWithValue("@PilotId", pilotId);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                    throw new Exception($"Pilot with PilotId: {pilotId}, Credits: {credits} failed to get updated.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating pilot credits: {ex.Message}");
        }

    }

    private int GetInventoryItemQuantity(SqliteConnection connection, int pilotId, int itemId)
    {
        int itemQt = 0;
        using (var command = connection.CreateCommand())
        {
            string pilotInv = pilotInventoryTableName;
            command.CommandText = 
                $"SELECT quantity " + 
                $"FROM {pilotInv} " + 
                $"WHERE pilot_id = @PilotId " + 
                $"AND item_id = @ItemId;";

            command.Parameters.AddWithValue("@PilotId", pilotId);
            command.Parameters.AddWithValue("@ItemId", itemId);

            using (IDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                    itemQt = reader.GetInt32(0);
            }
        }
        return itemQt;
    }

    private void DeleteInventoryItem(SqliteConnection connection, int pilotId, int itemId)
    {
        try
        {
            using (var command = connection.CreateCommand())
            {
                string pilotInv = pilotInventoryTableName;

                command.CommandText = 
                $"DELETE FROM {pilotInv} " + 
                $"WHERE pilot_id = @PilotId " + 
                $"AND item_id = @ItemId;";

                command.Parameters.AddWithValue("@PilotId", pilotId);
                command.Parameters.AddWithValue("@ItemId", itemId);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                    throw new Exception($"Inv item with PilotId: {pilotId}, ItemId: {itemId} not found. Nothing was deleted.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting shop item: {ex.Message}");
        }
    }

    private void InsertInventoryItem(SqliteConnection connection, int pilotId, int itemId, int quantity)
    {
        try
        {
            using (var command = connection.CreateCommand())
            {
                string pilotInv = pilotInventoryTableName;

                command.CommandText = 
                $"INSERT INTO {pilotInv} (pilot_id, item_id, quantity) " +
                $"VALUES (@PilotId, @ItemId, @Quantity);";

                command.Parameters.AddWithValue("@PilotId", pilotId);
                command.Parameters.AddWithValue("@ItemId", itemId);
                command.Parameters.AddWithValue("@Quantity", quantity);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                    throw new Exception($"Inv item with PilotId: {pilotId}, ItemId: {itemId} failed to get inserted.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error inserting inventory item: {ex.Message}");
        }
    }


    private void UpdateInventoryItemQuantity(SqliteConnection connection, int pilotId, int itemId, int quantity)
    {
        try
        {
            using (var command = connection.CreateCommand())
            {
                string pilotInv = pilotInventoryTableName;

                command.CommandText = 
                $"UPDATE {pilotInv} " +
                $"SET quantity = @Quantity " + 
                $"WHERE pilot_id = @PilotId " + 
                $"AND item_id = @ItemId;";

                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@PilotId", pilotId);
                command.Parameters.AddWithValue("@ItemId", itemId);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                    throw new Exception($"Inv item with PilotId: {pilotId}, ItemId: {itemId} failed to get updated.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating shop item: {ex.Message}");
        }
    }

    private int OTI(object obj)
    {
        return Int32.Parse(obj.ToString());
    }

    private float OTF(object obj)
    {
        return float.Parse(obj.ToString());
    }

}
