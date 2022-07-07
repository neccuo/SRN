using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemManager : MonoBehaviour
{
    [SerializeField] public int currentSystemID = 0;
    [SerializeField] private GameObject _planets;
    [SerializeField] private Text _systemNameDisplay;
    [SerializeField] private GameObject _spaceBG;


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

        SetBackground(newSystemID);
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

    private void SetBackground(int newSystemID)
    {
        SpriteRenderer sr = _spaceBG.GetComponent<SpriteRenderer>();
        string directory = "Sprites/Environment/SpaceBG/Space_Background_"; /*insert number*/
        sr.sprite = Resources.Load<Sprite>("" + directory + newSystemID.ToString());
        switch(newSystemID)
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

    /*private void SetPlanets()
    {
        _planetManager.PrepareSystemPlanets();
    }*/

}
