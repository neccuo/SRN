using UnityEngine;
using System.Data;
using System;

using Mono.Data.Sqlite;
using System.Collections.Generic;

public class SystemTEMP
{
    public int id { get; set; }
    public string name { get; set; }
    public int sun_id { get; set; }
    public int background_id { get; set; }
}

public class SystemDB : MonoBehaviour
{
    

    [SerializeField] private string _dbName = "URI=file:System.db";

    [SerializeField] private PlanetManager _planetManager;

    [SerializeField] private SystemManager _systemManager;



    void Start()
    {
        // CreateDB();

        // AddPlanet("Planet_0", 50.0f, 90.0f);

        // DisplayPlanet();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            SavePlanets();
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            for(int i = 0; i < 10; i++)
            {
                AddRandomPlanet();
            }
        }
    }

    // ONLY USE IT AT THE START
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

                    system.id = StrToInt(reader["id"].ToString());
                    system.name = reader["name"].ToString();
                    system.sun_id = StrToInt(reader["sun_id"].ToString());
                    system.background_id = StrToInt(reader["background_id"].ToString());

                    reader.Close();
                }
            }
            connection.Close();
        }
        return system;
    }

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

    private int StrToInt(string str)
    {
        return Int32.Parse(str);
    }
}
