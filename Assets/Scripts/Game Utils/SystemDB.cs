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
        LoadPlayer();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            SavePlanets();
            SavePlayer();
            UpdatePrices();
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            // LoadPlayer();
        }
    }

    // ONLY USE IT AT THE START
    // ***planets***
    public void LoadPlanets()
    {
        // looks shit, change it later
        using (var connection = new SqliteConnection(_dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM planets;";

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _planetManager.SpawnPlanet(reader);
                    }
                    reader.Close();
                }
            }
            connection.Close();
        }
    }

    // USE IT WHEN CHANGING SYSTEMS
    // ***system_planets***
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
                command.CommandText = "SELECT planet_id FROM system_planets WHERE system_id = '" + systemID + "';";

                using (IDataReader reader = command.ExecuteReader())
                {
                    int num;
                    while (reader.Read())
                    {
                        num = Int32.Parse(reader["planet_id"].ToString());
                        idList.Add(num);
                    }
                    reader.Close();
                }
            }
            connection.Close();
        }
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
        $"angle = {pilot.angle}, system_id = {pilot.system_id} WHERE id = 0";
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
        command.ExecuteNonQuery();

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
