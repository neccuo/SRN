using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class SystemManager : MonoBehaviour
{
    [SerializeField] public int currentSystemID = 0;
    [SerializeField] private Text _systemNameDisplay;
    [SerializeField] private GameObject _spaceBG;
    [SerializeField] private SystemDB _systemDB;
    [SerializeField] private PlanetManager _planetManager;

    void Awake()
    {
        // _planetManager = _planets.GetComponent<PlanetManager>();
    }

    void Start()
    {
        ChangeSystem(1); // START WHERE YOU LEFT OFF?
    }

    void Update()
    {
        
    }

    public void ChangeSystem(int newSystemID)
    {
        // check for validity maybe??
        SystemTEMP sysTemp = _systemDB.GetSystemData(newSystemID);

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
            case 2:
                sr.sprite = Resources.Load<Sprite>(directory + 2.ToString());
                sr.color = new Color32(231, 231, 231, 255);
                break;
            default:
                sr.sprite = Resources.Load<Sprite>(directory + 1.ToString());
                sr.color = new Color32(103, 103, 103, 255);
                break;

        }
    }

    private int StrToInt(string str)
    {
        return Int32.Parse(str);
    }


}
