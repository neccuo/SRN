using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public enum GameState
{
    PlanMovement,
    DuringMovement,
    Combat,
    TurnEvaluation,
    CheatBarState,
    ShopState,
    SpaceSystemLoad
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject cheatCodeBar;
    public GameObject shopBar;

    public Player player;

    private Controller _controllerGod;


    // public GameObject portalManager;

    public PortalManager portalManager;

    private GameState _state;

    public float GAMESPEED = 1;

    private void Awake()
    {
        Instance = this;
        ChangeGameState(GameState.PlanMovement);
    }

    void Start()
    {
        _controllerGod = Controller.ControllerGod;
    }

    void Update()
    {
        HandleGameState();
    }

    public GameState GetCurrentState()
    {
        return _state;
    }

    void HandleGameState()
    {
        switch (GetCurrentState())
        {
            case GameState.PlanMovement:
                _controllerGod.TakePlanMovementInput(); // where state changes may happen
                break;

            case GameState.DuringMovement:
                if(player.GetFollowedObject() != null)
                {
                    player.SetMovementFollow();
                }
                player.HandleMovement();
                // TakeDuringMovementInput(); // where state changes may happen
                _controllerGod.DuringMovementEndConditions();
                break;

            case GameState.CheatBarState:
                break;
            
            case GameState.ShopState:
                break;

            case GameState.SpaceSystemLoad:
                break;

            default:
                throw new MissingComponentException("" + GetCurrentState().ToString() + "is not an available state.");
        }
        // FinalStateChecker();
    }

    public void ChangeGameState(GameState newState) //
    {
        /// MAY NEED OPTIMIZATION, BUT NOT TODAY :3
        

        if(newState != _state)
        {
            // Debug.Log("Changing from state: " + _state.ToString() + " to state: " + newState.ToString());
        }

        GameState oldState = _state;
        _state = newState;

        switch (newState)
        {
            case GameState.PlanMovement:
                TraverseNpcs();
                Time.timeScale = 0;
                break;
            case GameState.DuringMovement:
                // YOU CAN ONLY ENTER GameState.DuringMovement from GameState.PlanMovement and vice versa.
                Time.timeScale = GAMESPEED;
                break;
            case GameState.Combat:
                break;
            case GameState.TurnEvaluation:
                break;
            case GameState.CheatBarState: // first, tell the cheat code the previous state. so, at the time of termination, return the state to it.
                cheatCodeBar.GetComponent<CheatCodeBarManager>().SetPreviousState(oldState); // reached its script
                cheatCodeBar.SetActive(true);
                break;
            case GameState.ShopState:
                Time.timeScale = 0; // BE CAREFUL ABOUT IT...!!!!
                shopBar.SetActive(true);
                // OpenShopMenu(Planet planet)
                break;
            case GameState.SpaceSystemLoad:
                portalManager.CreateSystemToTravel();
                break;
            default:
                throw new MissingComponentException("" + newState.ToString() + "is not an available state.");
        }
    }

    // MAYBE STORE EVERY IEnumerator FUNC TO A DEDICATED PLACE
    IEnumerator SavePos()
    {
        WWWForm form = new WWWForm();
        form.AddField("data", "asdf");
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/savepos.php", form);
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("register: success");
        }
        else
        {
            Debug.Log("register: fail");
            Debug.Log(www.error);
            Debug.Log(www.result);
        }
    }

    private class MiniNpc
    {
        public int id;
        public float x;
        public float y;

        public MiniNpc(int id, float x, float y)
        {
            this.id = id;
            this.x = x;
            this.y = y;
        }
    }


    void TraverseNpcs()
    {
        string jsonString = "";
        int idRegister;
        foreach(Transform child in GameObject.Find("NPCs").transform)
        {
            idRegister = child.gameObject.GetComponent<NPC>().GetNPCID();
            jsonString += JsonUtility.ToJson(new MiniNpc(idRegister, child.position.x, child.position.y));
        }
        Debug.Log(jsonString);
    }
}