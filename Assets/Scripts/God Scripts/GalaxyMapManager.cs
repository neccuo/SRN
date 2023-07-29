using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapStar
{
    public int id { get; set; }
    public string name { get; set; }

    // DISTANCE FROM THE CURRENT STAR, JUST AN IDEA...
    // public int dist { get; set; }

    // TO CALCULATE THE DISTANCE BETWEEN THE STARS, FUEL SHIT
    // public Vector2 coors;
}

// MUHTEMELEN CANVASA COMPONENT OLARAK KOYACAGIM
public class GalaxyMapManager : MonoBehaviour
{
    [SerializeField] public GameObject starListObject;
    [SerializeField] private GameObject _displayIcon;
    [SerializeField] private Button _confirmButton;

    [SerializeField] private SystemManager _systemManager;
    [SerializeField] private PortalManager _portalManager;


    private GameObject _currentStar = null;
    private GameObject _selectedStar = null;

    // MOSTLY GameState.PlanMovement but other possibilities like PlanetState may exist in the future
    private GameState stateToReturn = GameState.NullState;
    private GameState GalaxyMapState = GameState.GameUI;



    public void OnStarClick(GameObject obj)
    {
        if(_currentStar != obj)
        {
            SelectObject(obj);
        }
        else
        {
            NullifySelection();
        }
    }

    public void OpenMap()
    {
        gameObject.SetActive(true);
    }

    // BOUND TO CLOSE BUTTON
    public void CloseMap()
    {
        // YOU MIGHT WANT TO CHANGE THE ORDERING (deactivate -> change state OR change state -> deactivate)
        gameObject.SetActive(false);
    }

    // BOUND TO CONFIRM BUTTON
    public void ConfirmSelection()
    {
        int dstID = _selectedStar.GetComponent<GalaxyMapContainer>().systemID;
        GameObject targetPortalObj = _portalManager.SystemIdToPortalObject(dstID);

        CloseMap();

        Controller.ControllerGod.SetFollowProcedure(targetPortalObj);

        Debug.Log(dstID.ToString());
    }


    // not well optimized
    void OnEnable() 
    {
        stateToReturn = GameManager.Instance.GetCurrentState();
        Controller.ControllerGod.ChangeState(GalaxyMapState);

        int currentSystemID = _systemManager.currentSystemID;
        foreach(Transform star in starListObject.transform)
        {
            GameObject starObj = star.gameObject;
            int childSysID = starObj.GetComponent<GalaxyMapContainer>().systemID;
            if(currentSystemID == childSysID)
            {
                _currentStar = starObj;
                starObj.transform.GetChild(0).GetComponent<Text>().color = new Color32(0, 255, 44, 255);
                break;
            }
        }
    }

    void OnDisable() 
    {
        _currentStar.transform.GetChild(0).GetComponent<Text>().color = Color.white;
        _currentStar = null;

        NullifySelection();

        Controller.ControllerGod.ChangeState(stateToReturn);
        stateToReturn = GameState.NullState;
    }

    private void NullifySelection()
    {
        _selectedStar = null;
        _confirmButton.interactable = false;
        _displayIcon.SetActive(false);
        _displayIcon.transform.position = new Vector3(0, 0, 0);
    }

    private void SelectObject(GameObject obj)
    {
        _selectedStar = obj;
            
        _displayIcon.transform.position = _selectedStar.transform.position;
        _displayIcon.SetActive(true);

        _confirmButton.interactable = true;
    }
}
