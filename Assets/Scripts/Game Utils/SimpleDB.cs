using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.Collections.Generic;

public class SimpleDB : MonoBehaviour
{
    [SerializeField] private string _dbName = "URI=file:System.db";

    [SerializeField] private PlanetManager _planetManager;


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
        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadPlanets();
            // load planets from DB
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            SavePlanets();
            // load planets from DB
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            for(int i = 0; i < 10; i++)
            {
                AddRandomPlanet();
            }
        }
    }

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

    public void SavePlanets()
    {
        using (var connection = new SqliteConnection(_dbName))
        {
            connection.Open();

            Dictionary<int, Vector2> planetDic = _planetManager.GetPlanetPoss();

            foreach(KeyValuePair<int, Vector2> entry in planetDic)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE planets SET " +
                        "x_axis = '" + entry.Value.x + "', " +
                        "y_axis = '" + entry.Value.y + "' " +
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
        string name = "Planet " + Random.Range(0, 10000).ToString();
        float x = Random.Range(-80.0f, 80.0f);
        float y = Random.Range(-80.0f, 80.0f);
        float scale = Random.Range(0.22f, 4.0f);
        float speed = Random.Range(-15.0f, 15.0f);

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
