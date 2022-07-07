using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemManager : MonoBehaviour
{
    [SerializeField] public int currentSystemID = 0;
    [SerializeField] private GameObject _planets;
    [SerializeField] private Text _systemNameDisplay;

    private PlanetManager _planetManager;

    void Awake()
    {
        _planetManager = _planets.GetComponent<PlanetManager>();
    }

    void Start()
    {
        ChangeSystem(0); // START WHERE YOU LEFT OFF?
    }

    void Update()
    {
        
    }

    public void ChangeSystem(int newSystemID)
    {
        // check for validity maybe??

        // SetBackground
        // SetSun
        _planetManager.PrepareSystemPlanets(currentSystemID, newSystemID);
        // SetNPCs
        currentSystemID = newSystemID;
        _systemNameDisplay.text = "!System!: " + newSystemID;
    }

    private string GetSystemNameByID()
    {
        return "";
    }

    /*private void SetPlanets()
    {
        _planetManager.PrepareSystemPlanets();
    }*/

}
