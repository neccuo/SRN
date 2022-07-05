using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

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

    public void AddPlanet(string name, float x_axis, float y_axis)
    {
        using (var connection = new SqliteConnection(_dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO planets (name, x_axis, y_axis) VALUES (" +
                    "'" + name + "', " +
                    "'" + x_axis + "', " +
                    "'" + y_axis + "'" +
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
