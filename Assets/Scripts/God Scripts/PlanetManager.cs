using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;


public class PlanetManager : MonoBehaviour
{
    [SerializeField] private GameObject _planetPrefab;
    [SerializeField] private SystemManager _systemManager;
    [SerializeField] private SystemDB _systemDB;

    // <systemId, Planet>
    [SerializeField] private Dictionary<int, List<Planet>> _planetsBySystemDic = new Dictionary<int, List<Planet>>();

    // needs: SetAllPlanets()
    private List<Planet> allPlanetsList;


    public GameObject GetClosestPlanetInSystem(int systemId, Vector3 origin)
    {
        List<Planet> systemPlanets = _planetsBySystemDic[systemId];
        GameObject chosenObject = null;

        float closestDist = -1f;
        float dist;
        foreach(Planet planet in systemPlanets)
        {
            GameObject tempObj = planet.gameObject;
            // distance from origin to tempObj (planet in our case) 
            dist = (origin - tempObj.transform.position).magnitude;
            if(dist < closestDist || closestDist == -1f)
            {
                closestDist = dist;
                chosenObject = tempObj;
            }
        }
        return chosenObject;
    }

    // ONLY FOR SYS
    // Remember, load before save
    public void SetAllPlanetsList()
    {
        allPlanetsList = new List<Planet>();
        foreach(Transform child in transform)
        {
            Planet planet = child.gameObject.GetComponent<Planet>();
            allPlanetsList.Add(planet);
        }
    }

    public List<Planet> GetAllPlanetsList()
    {
        return allPlanetsList;
    }

    public void PrintPlanetStockById(int id)
    {
        var stockList = _systemDB.GetShopItemsByShopId(id);
        foreach(var item in stockList)
        {
            Debug.Log(item.name);
        }
    }

    public void PrepareSystemPlanets(int oldSysID, int newSysID)
    {
        if(oldSysID > 0)
        {
            List<Planet> deactivatePlanetList = _planetsBySystemDic[oldSysID];
            foreach(Planet planet in deactivatePlanetList)
            {
                planet.gameObject.SetActive(false);
            }
        }

        List<Planet> activatePlanetList = _planetsBySystemDic[newSysID];
        foreach(Planet planet in activatePlanetList)
        {
            planet.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        UpdatePlanetPoss();
    }

    public void SpawnPlanet(PlanetTEMP temp)
    {
        GameObject newPlanet = Instantiate(_planetPrefab);
        int id = temp.id;
        int sys_id = temp.system_id;
        newPlanet.transform.parent = this.transform;

        Planet planet = newPlanet.GetComponent<Planet>().SetPlanet(
            id, 
            temp.name,
            temp.x_axis,
            temp.y_axis,
            temp.scale,
            temp.angular_speed,
            sys_id,
            temp.sprite_id,
            temp.shop_id
        );
        AddPlanetToSystemDic(sys_id, planet);
    }

    public Vector3 PickRandomPlanetPos()
    {
        int childCount = transform.childCount;
        if(childCount == 0)
        {
            Debug.LogError("Planet count is 0, can't pick a planet");
            return new Vector3(0,0,-100);
        }
        int rndNum = UnityEngine.Random.Range(0, childCount);
        Transform chosenPlanet = transform.GetChild(rndNum);
        Debug.Log(chosenPlanet.name + " is chosen. Will spawn in: " + chosenPlanet.position);
        return chosenPlanet.position;
    }

    private void AddPlanetToSystemDic(int systemId, Planet newPlanet)
    {
        if (_planetsBySystemDic.ContainsKey(systemId))
        {
            _planetsBySystemDic[systemId].Add(newPlanet);
        }
        else
        {
            List<Planet> newPlanetList = new List<Planet>();
            newPlanetList.Add(newPlanet);
            _planetsBySystemDic[systemId] = newPlanetList;
        }
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
        }
    }
}
