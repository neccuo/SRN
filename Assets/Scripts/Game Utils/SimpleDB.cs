using UnityEngine;
using System.Data;
using System;

using Mono.Data.Sqlite;
using System.Collections.Generic;

public class SimpleDB : MonoBehaviour
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
        if(Input.GetKeyDown(KeyCode.C))
        {
            CreateDB();
        }
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
        if(Input.GetKeyDown(KeyCode.G))
        {
            GetPlanetsBySystemID(_systemManager.currentSystemID);
        }
    }

    // ONLY USE IT AT THE START
    public void LoadPlanets()
    {
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


    public void KillDB()
    {
        using (var connection = new SqliteConnection(_dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DROP TABLE planets";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    public void CreateDB()
    {
        using (var connection = new SqliteConnection(_dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS planets (" +
                    "name VARCHAR(20)," +
                    "x_axis FLOAT," +
                    "y_axis FLOAT" +
                    ");";
                command.ExecuteNonQuery();
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

    public void DisplayPlanet()
    {
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
                        Debug.Log("Name: " + reader["name"]);
                    }
                    reader.Close();
                }
            }
            connection.Close();
        }
    }
}
