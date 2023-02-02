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

    // [SerializeField] private Dictionary<int, Vector2> planetPoss = new Dictionary<int, Vector2>();

    [SerializeField] private Dictionary<int, Planet> _planetDic = new Dictionary<int, Planet>();

    void Awake()
    {
        // _systemDB.LoadPlanets();
    }

    void Start()
    {
    }

    public void PrintPlanetStockById(int id)
    {
        _systemDB.GetShopStockById(id);
    }

    public void PrepareSystemPlanets(int oldSysID, int newSysID)
    {
        List<int> deactivateIdList = _systemDB.GetPlanetsBySystemID(oldSysID);
        List<int> activateIdList = _systemDB.GetPlanetsBySystemID(newSysID);

        foreach(int i in deactivateIdList)
        {
            _planetDic[i].gameObject.SetActive(false);
        }
        foreach(int i in activateIdList)
        {
            _planetDic[i].gameObject.SetActive(true);
        }


    }

    private void FillPlanetDic(int id, Planet planet)
    {
        _planetDic[id] = planet;
    }


    void Update()
    {
        UpdatePlanetPoss();
        if(Input.GetKeyDown(KeyCode.D))
        {
            foreach(KeyValuePair<int, Planet> row in _planetDic)
            {
                Debug.Log(row.Key);
            }
        }
    }

    public void SpawnPlanet(PlanetTEMP temp)
    {
        GameObject newPlanet = Instantiate(_planetPrefab);
        int id = temp.id;
        newPlanet.transform.parent = this.transform;

        Planet planetLogic = newPlanet.GetComponent<Planet>().SetPlanet(
            id, 
            temp.name,
            temp.x_axis,
            temp.y_axis,
            temp.scale,
            temp.angular_speed,
            temp.system_id,
            temp.sprite_id,
            temp.shop_id
        );
        FillPlanetDic(id, planetLogic);
    }

    public void SetPlanetTexture(int id)
    {
        GameObject planetObject = _planetDic[id].gameObject;
        // SpriteRenderer

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

    /*public Planet GetPlanetByID(int id)
    {
        return null;
    }*/

}
