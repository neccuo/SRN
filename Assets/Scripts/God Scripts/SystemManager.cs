using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class SystemManager : MonoBehaviour
{
    // 0 is a default and an invalid value
    [SerializeField] public int currentSystemID = 0;
    [SerializeField] private Text _systemNameDisplay;
    [SerializeField] private GameObject _spaceBG;
    [SerializeField] private SystemDB _systemDB;
    [SerializeField] private PlanetManager _planetManager;
    [SerializeField] private NpcManager _npcManager;


    void Awake()
    {
        // _planetManager = _planets.GetComponent<PlanetManager>();
    }

    void Start()
    {
        // ChangeSystem(1); // START WHERE YOU LEFT OFF?
    }

    void Update()
    {
        
    }

    public void ChangeSystem(int newSystemID)
    {
        // check for validity maybe??
        SystemTEMP sysTemp = _systemDB.GetSystemData(newSystemID);

        _npcManager.PrepareSystemNPCs(currentSystemID, newSystemID);
        _planetManager.PrepareSystemPlanets(currentSystemID, newSystemID);
        SetBackground(sysTemp.background_id);
        SetSun(sysTemp.sun_id);
        // SetNPCs
        currentSystemID = sysTemp.id;
        _systemNameDisplay.text = "!System!: " + sysTemp.name;
    }

    private void SetSun(int id)
    {

    }

    private void SetBackground(int id)
    {
        SpriteRenderer sr = _spaceBG.GetComponent<SpriteRenderer>();
        string directory = "Sprites/Environment/SpaceBG/Space_Background_"; /*insert number*/
        sr.sprite = Resources.Load<Sprite>("" + directory + id.ToString());
        switch(id)
        {
            case 1:
                sr.color = new Color32(183, 183, 183, 255);
                break;
            case 2:
                sr.color = new Color32(118, 118, 118, 255);
                break;
            case 3:
                sr.color = new Color32(255, 255, 255, 255);
                break;
            default:
                sr.color = new Color32(183, 183, 183, 255);
                break;

        }
    }

    private int StrToInt(string str)
    {
        return Int32.Parse(str);
    }


}
