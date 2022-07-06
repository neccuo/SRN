using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;


public class PlanetManager : MonoBehaviour
{
    [SerializeField] private GameObject _planetPrefab;
    [SerializeField] private SimpleDB _simpleDB;

    // [SerializeField] private Dictionary<int, Vector2> planetPoss = new Dictionary<int, Vector2>();

    [SerializeField] private Dictionary<int, Planet> _planetDic = new Dictionary<int, Planet>();

    void Start()
    {
        _simpleDB.LoadPlanets();
        // fill planet dic
    }

    private void FillPlanetDic(int id, Planet planet)
    {
        _planetDic[id] = planet;
    }


    void Update()
    {
        UpdatePlanetPoss();
    }

    public void SpawnPlanet(IDataReader data)
    {
        GameObject newPlanet = Instantiate(_planetPrefab);
        int id = ObjectToInt(data["id"]);
        newPlanet.transform.parent = this.transform;

        Planet planetObj = newPlanet.GetComponent<Planet>().SetPlanet(
            id, 
            data["name"].ToString(),
            ObjectToFloat(data["x_axis"]), 
            ObjectToFloat(data["y_axis"]), 
            ObjectToFloat(data["scale"]), 
            ObjectToFloat(data["angular_speed"])
        );
        FillPlanetDic(id, planetObj);
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
            //_planetDic[id] = planet;
            //planetPoss[id] = (Vector2) child.position;
        }
    }

    // TODO: OPTIMIZATION
    /*public Dictionary<int, Vector2> GetPlanetPoss()
    {
        return planetPoss;
    }*/

    public Dictionary<int, Planet> GetPlanetDic()
    {
        return _planetDic;
    }

    private int ObjectToInt(object obj)
    {
        return Int32.Parse(obj.ToString());
    }

    private float ObjectToFloat(object obj)
    {
        return float.Parse(obj.ToString());
    }

    public Planet GetPlanetByID(int id)
    {
        return null;
    }

}