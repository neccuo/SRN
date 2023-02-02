using UnityEngine;
using System.Data;
using System;
using Mono.Data.Sqlite;
using System.Collections.Generic;

// ---> NOTES TO SELF <---
// At some point, you will have to implement a proper save/load system where
// every single aspect of the game has to be saved synchronously. Which means
// you have to revert every change if something goes wrong (including outside commands).
// Also, decide when to save. End of the day or when the game stops.


public class SystemTEMP
{
    public int id { get; set; }
    public string name { get; set; }
    public int sun_id { get; set; }
    public int background_id { get; set; }
}

public class PlanetTEMP
{
    public int id { get; set; }
    public string name { get; set; }
    public float x_axis { get; set; }
    public float y_axis { get; set; }
    public float scale { get; set; }
    public float angular_speed { get; set; }
    public int system_id { get; set; }
    public int sprite_id { get; set; }
    public int shop_id { get; set; }

}

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


public class SystemDB : MonoBehaviour
{
    [SerializeField] private string _dbName = "URI=file:System.db";
    [SerializeField] private PlanetManager _planetManager;
    [SerializeField] private SystemManager _systemManager;
    [SerializeField] private NpcManager _npcManager;
    [SerializeField] private Player _player;





    void Start()
    {
        LoadPlanets();
        LoadPilots();

        LoadPlayer(); // THIS ONE ALSO SETS THE SYSTEM
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            // Save time/date
            SavePlanets();
            SavePilots();
            SavePlayer();
            UpdatePrices();
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            // LoadPlayer();
        }
    }


    // IT ONLY PRINTS!!!!!!!!
    public void GetShopStockById(int id)
    {
        SqliteConnection connection = new SqliteConnection(_dbName);
        connection.Open();

        string shopInventoryTableName = "shop_inventories";

        string ans = "";

        using (var command = connection.CreateCommand())
        {
            command.CommandText = 
            $"SELECT items.name, ROUND(items.price * {shopInventoryTableName}.price_coefficient), {shopInventoryTableName}.quantity " + 
            $"FROM items " +
            $"JOIN {shopInventoryTableName} ON {shopInventoryTableName}.item_id = items.id " +
            $"WHERE {shopInventoryTableName}.shop_id = {id};";

            using (IDataReader reader = command.ExecuteReader())
            {
                string str = "";
                while (reader.Read()) 
                {
                    for (int i = 0; i < reader.FieldCount; i++) 
                    {
                        str += reader[i].ToString();
                        str += ", ";
                    }
                    str += "\n";
                }
                ans = str;
            }
        }

        Debug.Log(ans);
    }

    // ONLY USE IT AT THE START
    // ***planets***
    public void LoadPlanets()
    {
        SqliteConnection connection = new SqliteConnection(_dbName);
        connection.Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT * FROM planets ORDER BY system_id;";

            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    PlanetTEMP temp = new PlanetTEMP();
                    temp.id = OTI(reader["id"]);
                    temp.name = reader["name"].ToString();
                    temp.x_axis = OTF(reader["x_axis"]);
                    temp.y_axis = OTF(reader["y_axis"]);
                    temp.scale = OTF(reader["scale"]);
                    temp.angular_speed = OTF(reader["angular_speed"]);
                    temp.system_id = OTI(reader["system_id"]);
                    temp.sprite_id = OTI(reader["sprite_id"]);
                    temp.shop_id = reader["shop_id"] != DBNull.Value ? OTI(reader["shop_id"]) : -1;

                    _planetManager.SpawnPlanet(temp);
                }
                reader.Close();
            }
        }
        connection.Close();
    }

    // USE IT WHEN CHANGING SYSTEMS
    // ***planets***
    public List<int> GetPlanetsBySystemID(int systemID)
    {
        List<int> idList = new List<int>();
        if(systemID == 0) // FOR TEST OR INIT
        {
            return idList;
        }
        using (var connection = new SqliteConnection(_dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id FROM planets WHERE system_id = '" + systemID + "';";

                using (IDataReader reader = command.ExecuteReader())
                {
                    int num;
                    while (reader.Read())
                    {
                        num = Int32.Parse(reader["id"].ToString());
                        idList.Add(num);
                    }
                    reader.Close();
                }
            }
            connection.Close();
        }
        return idList;
    }
    // ***pilots***
    public List<int> GetPilotsBySystemID(int systemID)
    {
        List<int> idList = new List<int>();
        if(systemID == 0) // FOR TEST OR INIT
        {
            return idList;
        }
        SqliteConnection connection = new SqliteConnection(_dbName);
        connection.Open();
        SqliteCommand command = connection.CreateCommand();

        command.CommandText = $"SELECT id FROM pilots WHERE system_id = {systemID} AND id != 0;";

        using (IDataReader reader = command.ExecuteReader())
        {
            int num;
            while (reader.Read())
            {
                num = OTI(reader["id"]);
                idList.Add(num);
            }
            reader.Close();
        }

        command.Dispose();
        connection.Close();
        return idList;
    }

    // ***systems***
    public SystemTEMP GetSystemData(int systemID)
    {
        SystemTEMP system = new SystemTEMP();
        if(systemID == 0) // FOR TEST OR INIT
        {
            return null;
        }
        using (var connection = new SqliteConnection(_dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                // EXPECTING 1 ROW
                command.CommandText = "SELECT * FROM systems WHERE id = '" + systemID + "';";
                using (IDataReader reader = command.ExecuteReader())
                {
                    reader.Read();

                    system.id = OTI(reader["id"]);
                    system.name = reader["name"].ToString();
                    system.sun_id = OTI(reader["sun_id"]);
                    system.background_id = OTI(reader["background_id"]);

                    reader.Close();
                }
            }
            connection.Close();
        }
        return system;
    }

    // ***planets***
    public void SavePlanets()
    {
        using (var connection = new SqliteConnection(_dbName))
        {
            connection.Open();
            Dictionary<int, Planet> planetDic = _planetManager.GetPlanetDic();

            foreach(KeyValuePair<int, Planet> entry in planetDic)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE planets SET " +
                        "x_axis = '" + entry.Value.transform.position.x + "', " +
                        "y_axis = '" + entry.Value.transform.position.y + "' " +
                        "WHERE id = '" + entry.Key + "'" +
                        ";";
                    command.ExecuteNonQuery();
                }
            }
            connection.Close();
        }
    }

    // USE "LOAD" AFTER IT
    // GOOD FOR BENCHMARK
    // ***planets***
    public void AddRandomPlanet()
    {
        string name = "Planet " + UnityEngine.Random.Range(0, 10000).ToString();
        float x = UnityEngine.Random.Range(-80.0f, 80.0f);
        float y = UnityEngine.Random.Range(-80.0f, 80.0f);
        float scale = UnityEngine.Random.Range(0.22f, 4.0f);
        float speed = UnityEngine.Random.Range(-15.0f, 15.0f);

        using (var connection = new SqliteConnection(_dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO planets (name, x_axis, y_axis, scale, angular_speed) VALUES (" +
                    "'" + name + "', " +
                    "'" + x + "', " +
                    "'" + y + "', " +
                    "'" + scale + "', " +
                    "'" + speed + "'" +
                    ");";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    // ***items***
    public void UpdatePrices()
    {
        SqliteConnection connection = new SqliteConnection(_dbName);
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        SqliteTransaction trans = connection.BeginTransaction();

        Dictionary<int, int> idDic = new Dictionary<int, int>();
        try
        {
            command.CommandText = "SELECT id, price FROM items;";
            using(IDataReader reader = command.ExecuteReader())
            {
                while(reader.Read())
                {
                    idDic[OTI(reader["id"])] = OTI(reader["price"]);
                }
            }
            int tempID = 0;
            int tempPrice = 0;
            string priceLog = "Dic:\n";
            foreach(KeyValuePair<int, int> pair in idDic)
            {
                tempID = pair.Key;
                tempPrice = NewPrice(pair.Value);
                command.CommandText = $"UPDATE items SET price = {tempPrice} WHERE id = {tempID};";
                command.ExecuteNonQuery();

                priceLog += $"key: {pair.Key}   value: {pair.Value}\n";
            }
            trans.Commit();
            // Debug.Log(priceLog);
        }
        catch(Exception e)
        {
            trans.Rollback();
            Debug.LogError(e.ToString());
            Debug.LogWarning("Price update failed, rollback");
        }
        finally
        {
            trans.Dispose();
            command.Dispose();
            connection.Close();
        }
    }

    // ***pilots
    public void LoadPilots()
    {
        SqliteConnection connection = new SqliteConnection(_dbName);
        connection.Open();
        SqliteCommand command = connection.CreateCommand();
        SqliteTransaction trans = connection.BeginTransaction();

        // not including 0
        command.CommandText = "SELECT * FROM pilots WHERE id != 0;";

        using(IDataReader reader = command.ExecuteReader())
        {
            // expecting only one iteration
            while(reader.Read())
            {
                PilotTEMP temp = new PilotTEMP();
                temp.id = OTI(reader["id"]);
                temp.name = reader["name"].ToString();
                temp.credits = OTI(reader["credits"]);
                temp.race = reader["race"].ToString();
                temp.hull_id = OTI(reader["hull_id"]);
                temp.x_axis = OTF(reader["x_axis"]);
                temp.y_axis = OTF(reader["y_axis"]);
                temp.angle = OTF(reader["angle"]);
                temp.system_id = OTI(reader["system_id"]);

                _npcManager.SpawnNPC(temp);
            }
        }
        trans.Dispose();
        command.Dispose();
        connection.Close();
    }

    public void SavePilots()
    {
        SqliteConnection connection = new SqliteConnection(_dbName);
        SqliteCommand command = connection.CreateCommand();
        connection.Open();

        Dictionary<int, NPC> dic = _npcManager.GetNpcDic();
        foreach(KeyValuePair<int, NPC> pair in dic)
        {
            NPC npc = pair.Value;
            PilotTEMP p = new PilotTEMP();
            p.id = npc.GetNPCID();
            p.credits = npc.GetNpcCredits();
            p.x_axis = npc.transform.position.x;
            p.y_axis = npc.transform.position.y;
            p.angle = npc.transform.rotation.eulerAngles.z;
            p.system_id = npc.GetSystemID();

            command.CommandText = $"UPDATE pilots SET credits = {p.credits}, x_axis = {p.x_axis}, y_axis = {p.y_axis}, " +
            $"angle = {p.angle}, system_id = {p.system_id} WHERE id = {p.id};";
            command.ExecuteNonQuery();

        }

        command.Dispose();
        connection.Close();
    }

    public void SavePlayer()
    {
        SqliteConnection connection = new SqliteConnection(_dbName);
        SqliteCommand command = connection.CreateCommand();
        connection.Open();

        PilotTEMP pilot = new PilotTEMP();
        pilot.credits = _player.GetPlayerCredits();
        pilot.x_axis = _player.transform.position.x;
        pilot.y_axis = _player.transform.position.y;
        pilot.angle = (_player.transform.rotation.eulerAngles.z);
        pilot.system_id = _systemManager.currentSystemID;

        command.CommandText = $"UPDATE pilots SET credits = {pilot.credits}, x_axis = {pilot.x_axis}, y_axis = {pilot.y_axis}, " +
        $"angle = {pilot.angle}, system_id = {pilot.system_id} WHERE id = 0;";
        command.ExecuteNonQuery();

        command.Dispose();
        connection.Close();
    }

    public void LoadPlayer()
    {
        SqliteConnection connection = new SqliteConnection(_dbName);
        SqliteCommand command = connection.CreateCommand();
        connection.Open();

        command.CommandText = $"SELECT * FROM pilots WHERE id = 0";
        // command.ExecuteNonQuery();

        using(IDataReader reader = command.ExecuteReader())
        {
            // expecting only one iteration
            while(reader.Read())
            {
                Transform tr = _player.transform;
                _player.SetPlayerCredits(OTI(reader["credits"]));
                tr.position = new Vector3(OTF(reader["x_axis"]), OTF(reader["y_axis"]), tr.position.z);
                tr.Rotate(tr.rotation.x, tr.rotation.y, OTF(reader["angle"]));
                // tr.rotation = new Quaternion(tr.rotation.x, tr.rotation.y, OTF(reader["angle"]), tr.rotation.w);
                _systemManager.ChangeSystem(OTI(reader["system_id"]));
            }
        }
        
        command.Dispose();
        connection.Close();
    }

    private void PrintDic<TKey, TValue>(Dictionary<TKey, TValue> dic)
    {
        string str = "Dic:\n";
        foreach(KeyValuePair<TKey, TValue> pair in dic)
        {
            str += $"key: {pair.Key}   value: {pair.Value}\n";
        }
        Debug.Log(str);
    }

    private int NewPrice(int currentPrice)
    {
        int retVal = currentPrice + UnityEngine.Random.Range(-99, 100);
        retVal = (retVal > 3000 || retVal < 50) ? currentPrice : retVal;
        return retVal;
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
