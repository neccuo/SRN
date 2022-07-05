using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;


public class PlanetManager : MonoBehaviour
{
    [SerializeField] private GameObject _planetPrefab;

    public void SpawnPlanet(IDataReader data)
    {
        GameObject newPlanet = Instantiate(_planetPrefab);
        newPlanet.transform.parent = this.transform;
        // Int32.Parse("1234"); öffffffffffffffffffffffffffffffffffffffffff
        newPlanet.GetComponent<Planet>().SetPlanet(
            ObjectToInt(data["id"]), 
            data["name"].ToString(),
            ObjectToFloat(data["x_axis"]), 
            ObjectToFloat(data["y_axis"]), 
            ObjectToFloat(data["scale"]), 
            ObjectToFloat(data["angular_speed"])
        );
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
