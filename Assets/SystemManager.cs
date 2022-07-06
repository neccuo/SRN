using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    [SerializeField] public int currentSystemID;
    [SerializeField] private GameObject _planets;


    public void ChangeSystem(int systemID)
    {
        // check for validity maybe??
        currentSystemID = systemID;
        // SetBackground
        // SetSun
        // SetPlanets
        // SetNPCs
    }

    private void SetPlanets()
    {

    }

}
