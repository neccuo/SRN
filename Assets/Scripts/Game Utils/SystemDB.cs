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

// ONLY MANAGERS CAN ACCESS IT
public class SystemDB : MonoBehaviour
{
    [SerializeField] private string _dbName = "URI=file:System.db";
    [SerializeField] private SystemManager _systemManager;
    [SerializeField] private NpcManager _npcManager;
    [SerializeField] private PlanetManager _planetManager;
    // [SerializeField] private ShopManager _shopManager;
    [SerializeField] private Player _player;

    private ShopUtilsDB _shopUtilsDB;
    private PilotUtilsDB _pilotUtilsDB;

    // AFERIN GUNU KURTARDIN AQ COCU
    // BIR ARA LIFE CYCLE'I DUZGUN PLANLA
    void Awake() 
    {
        _shopUtilsDB = new ShopUtilsDB();
        _pilotUtilsDB = new PilotUtilsDB();
    }

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

    // pilotId=0 for player
    public int GetPlayerCredits()
    {
        int credits = -1;
        using (var connection = new SqliteConnection(_dbName))
        {
            connection.Open();
            credits = _pilotUtilsDB.GetPilotCredit(connection, 0);
        }
        return credits;
    }

    // pilotId=0 means pilot is the player
    public bool BuyItem(int shopId, int itemId, int pilotId=0)
    {
        try
        {
            if(shopId < 0 || itemId < 0)
                throw new Exception($"SOMETHING IS WRONG w/ 'BuyItem'. shopId: {shopId}, itemId: {itemId}");

            using (var connection = new SqliteConnection(_dbName))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    // CHECK ASPECT
                    int price = _shopUtilsDB.GetPrice(connection, shopId, itemId);
                    int pilotCredit = _pilotUtilsDB.GetPilotCredit(connection, pilotId);

                    int newAmt = pilotCredit - price;

                    if (newAmt < 0)
                        throw new Exception($"ERROR: price ({price}) exceeds the pilotCredit ({pilotCredit})");

                    // SHOP ASPECT
                    _shopUtilsDB.ShopBuy(connection, shopId, itemId);

                    // PILOT GIVES CREDITS ASPECT
                    _pilotUtilsDB.SetPilotCredits(connection, pilotId, newAmt);

                    // PILOT INVENTORY ASPECT
                    _pilotUtilsDB.AddToInventory(connection, pilotId, itemId);

                    transaction.Commit();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error buying itemId[{itemId}] from shopId[{shopId}] to pilotId[{pilotId}]: {ex.Message}");
            return false;
        }
        return true;
    }

    public List<ShopItem> GetShopItemsByShopId(int id)
    {
        SqliteConnection connection = new SqliteConnection(_dbName);
        connection.Open();

        List<ShopItem> shopItemList = _shopUtilsDB.GetAllShopItems(connection, id);

        connection.Close();

        return shopItemList;
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

    // organized by chatgpt. 7/18/2023, 6:49 EST
    public void UpdatePrices()
    {
        Dictionary<int, int> idDic = new Dictionary<int, int>();

        using (SqliteConnection connection = new SqliteConnection(_dbName))
        {
            connection.Open();
            using (SqliteTransaction trans = connection.BeginTransaction())
            {
                try
                {
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT id, price FROM items;";

                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = Convert.ToInt32(reader["id"]);
                                int price = Convert.ToInt32(reader["price"]);
                                idDic[id] = price;
                            }
                        }
                    }

                    string priceLog = "Dic:\n";
                    foreach (KeyValuePair<int, int> pair in idDic)
                    {
                        int itemId = pair.Key;
                        int currentPrice = pair.Value;
                        int newPrice = NewPrice(currentPrice);

                        using (SqliteCommand updateCommand = connection.CreateCommand())
                        {
                            updateCommand.CommandText = $"UPDATE items SET price = {newPrice} WHERE id = {itemId};";
                            updateCommand.ExecuteNonQuery();
                        }

                        priceLog += $"key: {itemId}   value: {currentPrice}\n";
                    }

                    trans.Commit();
                    // Debug.Log(priceLog);
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    Debug.LogError(e.ToString());
                    Debug.LogWarning("Price update failed, rollback");
                }
            }
        }
    }

    // ***pilots
    // only npcs
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
        // pilot.credits = _player.GetPlayerCredits();
        pilot.credits = GetPlayerCredits();
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

    public int GetPlayerSystem()
    {
        int ans = -1;
        using (SqliteConnection connection = new SqliteConnection(_dbName))
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                    $"SELECT system_id " +
                    $"FROM pilots " +
                    $"WHERE id = 0;";

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        ans = reader.GetInt32(0);
                    else
                        throw new Exception($"READ UNSUCCESSFUL");
                }
            }
        }
        return ans;
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
                // PLAYER CREDIT SET IS HANDLED SOMEWHERE ELSE
                // _player.SetPlayerCredits(OTI(reader["credits"]));
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
