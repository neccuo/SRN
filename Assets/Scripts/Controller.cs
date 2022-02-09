using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    /// PlanMovement
    /// DuringMovement
    private GameManager gameInstance; // for storing the game state
    public Player player;

    void Start()
    {
        gameInstance = GameManager.Instance;
    }

    void Update()
    {
        HandleGameState();
        
    }

    void HandleGameState()
    {
        switch (gameInstance.State)
        {
            case GameState.PlanMovement:
                TakePlanMovementInput(); // where state changes may happen
                break;

            case GameState.DuringMovement:
                player.HandleMovement();
                TakeDuringMovementInput(); // where state changes may happen
                break;

            default:
                throw new MissingComponentException("" + gameInstance.ToString() + "is not an available state.");
        }
    }

    void TakePlanMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            gameInstance.UpdateGameState(GameState.DuringMovement); // continue game
        else if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            player.SetMovement();
    }

    void TakeDuringMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            gameInstance.UpdateGameState(GameState.PlanMovement); // stop game
        // else if()
    }

}
