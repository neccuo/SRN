using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    PlanMovement,
    DuringMovement,
    Combat,
    TurnEvaluation,
    MenuState
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState state;

    private void Awake()
    {
        Instance = this;
        UpdateGameState(GameState.PlanMovement);
    }

    public void UpdateGameState(GameState newState)
    {
        /// MAY NEED OPTIMIZATION, BUT NOT TODAY :3
        
        /*if(newState == state)
        {
            Debug.Log("Already in the state: (" + state.ToString() + ")");
            return;
        }*/

        if(newState != state)
            Debug.Log("Changing from state: " + state.ToString() + " to state: " + newState.ToString());
        state = newState;

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
            case GameState.MenuState:
                break;
            default:
                throw new MissingComponentException("" + newState.ToString() + "is not an available state.");
        }
    }
}