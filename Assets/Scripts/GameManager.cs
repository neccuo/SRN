using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    PlanMovement,
    DuringMovement,
    Combat,
    TurnEvaluation
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;

    private void Awake()
    {
        Instance = this;
        UpdateGameState(GameState.PlanMovement);
    }

    public void UpdateGameState(GameState newState)
    {
        Debug.Log("Changing from state: " + State.ToString() + " to state: " + newState.ToString());


        State = newState;


        switch (newState)
        {
            case GameState.PlanMovement:
                Time.timeScale = 0;
                break;
            case GameState.DuringMovement:
                Time.timeScale = 1;
                break;
            case GameState.Combat:
                break;
            case GameState.TurnEvaluation:
                break;
            default:
                throw new MissingComponentException("" + newState.ToString() + "is not an available state.");
        }
    }
}