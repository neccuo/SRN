using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    /// PlanMovement
    /// DuringMovement
    private GameManager gameInstance; // for storing the game state
    public Player player;

    public Vector2 playerTargetLocation;
    public Vector2 playerCurrentLocation;

    public ClickPoint clickPoint;

    float lastClickTime;

    public GameObject cheatCodeBar;

    void Start()
    {
        gameInstance = GameManager.Instance;

        playerCurrentLocation = player.transform.position;
        clickPoint.origin = playerCurrentLocation;

        // cheatCodeBar = GameObject.Find("CheatCodeBar");
    }

    void Update()
    {
        HandleGameState();
        clickPoint.origin = playerCurrentLocation;
        clickPoint.target = playerTargetLocation;
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
                playerCurrentLocation = player.HandleMovement();
                TakeDuringMovementInput(); // where state changes may happen
                break;

            default:
                throw new MissingComponentException("" + gameInstance.ToString() + "is not an available state.");
        }
        FinalStateChecker();
    }

    void ChangeState(GameState newState)
    {
        GameState stateToBeChanged = gameInstance.state;
        if(stateToBeChanged == GameState.PlanMovement)
        {
            clickPoint.DestroyArrow();
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
            playerTargetLocation = player.SetMovement();
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
        //GameObject.Find("CheatCodeBar").SetActive(true);
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
        if(playerTargetLocation == playerCurrentLocation && gameInstance.state != GameState.PlanMovement)
            ChangeState(GameState.PlanMovement); // stop game
    }

}
