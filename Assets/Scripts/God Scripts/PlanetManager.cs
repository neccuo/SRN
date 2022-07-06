using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;


public class PlanetManager : MonoBehaviour
{
    [SerializeField] private GameObject _planetPrefab;
    [SerializeField] private Dictionary<int, Vector2> planetPoss = new Dictionary<int, Vector2>();

    void Update()
    {
        UpdatePlanetPoss();

    }

    public void SpawnPlanet(IDataReader data)
    {
        GameObject newPlanet = Instantiate(_planetPrefab);
        newPlanet.transform.parent = this.transform;
        newPlanet.GetComponent<Planet>().SetPlanet(
            ObjectToInt(data["id"]), 
            data["name"].ToString(),
            ObjectToFloat(data["x_axis"]), 
            ObjectToFloat(data["y_axis"]), 
            ObjectToFloat(data["scale"]), 
            ObjectToFloat(data["angular_speed"])
        );
    }

    private void UpdatePlanetPoss()
    {
        int id;
        GameObject gObject;
        Planet planet;
        foreach(Transform child in transform)
        {
            gObject = child.gameObject;
            planet = gObject.GetComponent<Planet>();
            planet.OrbitSun();
            id = gObject.GetComponent<Planet>().GetPlanetID();
            planetPoss[id] = (Vector2) child.position;
        }
    }

    // TODO: OPTIMIZATION
    public Dictionary<int, Vector2> GetPlanetPoss()
    {
        return planetPoss;
    }

    private int ObjectToInt(object obj)
    {
        return Int32.Parse(obj.ToString());
    }

    private float ObjectToFloat(object obj)
    {
        return float.Parse(obj.ToString());
    }

}
