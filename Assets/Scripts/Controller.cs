using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    /// PlanMovement
    /// DuringMovement

    public static Controller ControllerGod; // GOD CLASS

    private GameManager gameInstance; // for storing the game state
    public Player player;

    // public Vector2 playerTargetLocation;
    // public Vector2 playerCurrentLocation;

    public ClickPoint clickPoint;

    float lastClickTime;

    public GameObject cheatCodeBar;

    private void Awake()
    {
        ControllerGod = this;
    }

    void Start()
    {
        gameInstance = GameManager.Instance;

        clickPoint.origin = (Vector2) player.transform.position;

        // cheatCodeBar = GameObject.Find("CheatCodeBar");
    }

    void Update()
    {
        HandleGameState();
        clickPoint.origin = (Vector2) player.transform.position;
        clickPoint.target = player.GetTarget();
    }

    private void WhenClicked() // Always use after click is used
    {
        lastClickTime = Time.time;
    }

    void HandleGameState()
    {
        switch (gameInstance.state)
        {
            case GameState.PlanMovement:
                TakePlanMovementInput(); // where state changes may happen
                break;

            case GameState.DuringMovement:
                player.HandleMovement();
                TakeDuringMovementInput(); // where state changes may happen
                break;

            case GameState.MenuState:
                break;

            

            default:
                throw new MissingComponentException("" + gameInstance.ToString() + "is not an available state.");
        }
        FinalStateChecker();
    }

    public void ChangeState(GameState newState) // DEFINITELY CLEAN IT IN THE FUTURE
    {
        GameState stateToBeChanged = gameInstance.state;
        if(stateToBeChanged == GameState.PlanMovement) // if you are going from PlanMovement, destroy arrow
        {
            clickPoint.DestroyArrow();
        }
        if(newState != GameState.DuringMovement) // if you are going to DuringMovement, reset the target of the player
        {
            player.ResetTarget();
        }
        gameInstance.UpdateGameState(newState);
    }

    void TakePlanMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ChangeState(GameState.DuringMovement); // continue game
        else if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            // THIS IS WHERE YOU LEFT DEBUGGING, KEEP UP
            // Debug.Log("YOYOYO HEY, LOOK: " + (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));

            WhenClicked();
            player.SetMovement();
            clickPoint.SpawnArrow();
        }
        else if (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.H)) // HOLD C AND PRESS H TO OPEN CHEAT CODE SCREEN
        {
            Debug.Log("Cheat code screen");
            OpenCheatCodeScreen();
        }
    }

    void OpenCheatCodeScreen()
    {
        // Debug.Log(cheatCodeBar.ToString());
        // GameObject.Find("CheatCodeBar").SetActive(true);
        // ChangeState(GameState.MenuState);
        cheatCodeBar.SetActive(true);
    }

    void TakeDuringMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ChangeState(GameState.PlanMovement); // stop game
        // else if()
    }

    void FinalStateChecker()
    {
        if(player.GetTarget() == (Vector2) player.transform.position && gameInstance.state == GameState.DuringMovement /*!= GameState.PlanMovement*/)
            ChangeState(GameState.PlanMovement); // stop game
    }

}
