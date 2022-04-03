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
    float doubleClickRate = 0.5f; // in seconds

    private void Awake()
    {
        ControllerGod = this;
    }

    void Start()
    {
        gameInstance = GameManager.Instance;

        clickPoint.origin = (Vector2) player.transform.position;

    }

    void Update()
    {
        // HandleGameState();
        clickPoint.origin = (Vector2) player.transform.position;
        clickPoint.target = player.GetTarget();
    }

    private bool CheckDoubleClick()
    {
        if(Time.unscaledTime - lastClickTime < doubleClickRate)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void WhenClicked() // Always use after click is used
    {
        lastClickTime = Time.unscaledTime;
    }

    /* void HandleGameState()
    {
        switch (gameInstance.GetCurrentState())
        {
            case GameState.PlanMovement:
                TakePlanMovementInput(); // where state changes may happen
                break;

            case GameState.DuringMovement:
                if(player.GetFollowedObject() != null)
                {
                    player.SetMovementFollow();
                }
                player.HandleMovement();
                // TakeDuringMovementInput(); // where state changes may happen
                DuringMovementEndConditions();
                break;

            case GameState.CheatBarState:
                break;
            
            case GameState.ShopState:
                break;

            case GameState.SpaceSystemLoad:
                break;

            default:
                throw new MissingComponentException("" + gameInstance.ToString() + "is not an available state.");
        }
        // FinalStateChecker();
    } */

    public void ChangeState(GameState newState) // DEFINITELY CLEAN IT IN THE FUTURE
    {
        GameState stateToBeChanged = gameInstance.GetCurrentState();
        if(stateToBeChanged == GameState.PlanMovement) // if you are going from PlanMovement, destroy arrow
        {
            clickPoint.DestroyArrow();
        }
        if(newState != GameState.DuringMovement) // if you are going to DuringMovement, reset the target of the player
        {
            player.ResetTarget();
            player.SetFollowedObject(null);
        }
        gameInstance.ChangeGameState(newState);
    }

    void BasicMovementProcedure()
    {
        player.SetMovementBasic();
        clickPoint.SpawnCrossArrow();
    }

    // hopefully will use algorithms at some point :P
    private bool _CheckDoubleClickWhiteList(string tag)
    {
        if(tag == "Planet" || tag == "Portal" || tag == "NPC")
        {
            return true;
        }
        return false;
    }

    public void TakePlanMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ChangeState(GameState.DuringMovement); // continue game
        else if (Input.GetMouseButtonDown(0)/* || Input.GetMouseButton(0)*/) 
        {
            // HOLDING MOUSE IS UNAVAILABLE FOR A WHILE
            if(!CheckDoubleClick()) // if it is first click
            {
                BasicMovementProcedure();
            }
            else // if double click
            {
                Vector2 mousePos2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit.collider != null && _CheckDoubleClickWhiteList(hit.collider.gameObject.tag)) // NEEDS SOME DETAILS IDENTIFYING THE COLLIDER
                {
                    Debug.Log(hit.collider.gameObject.name + " was double clicked");
                    player.SetFollowedObject(hit.collider.gameObject);
                    player.SetMovementFollow();
                    clickPoint.SpawnFollowArrow();
                }
                else /***NESTED IF STATEMENTS ARE SHITTY HACK, CHANGE AT SOME POINT***/
                {
                    BasicMovementProcedure();
                }
                Debug.Log("double clicked");
            }
            lastClickTime = Time.unscaledTime; // SAVE THE TIME WHEN IT IS CLICKED
        }
        else if (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.H)) // HOLD C AND PRESS H TO OPEN CHEAT CODE SCREEN
        {
            ChangeState(GameState.CheatBarState);
        }
    }

    public void DuringMovementEndConditions()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // SPACE IS CLICKED: normal pause
            ChangeState(GameState.PlanMovement); // stop game
        else if(player.GetTarget() == (Vector2) player.transform.position) // REACHED TO THE DESTINATION
        {
            if(player.GetFollowedObject() == null)
            {
                ChangeState(GameState.PlanMovement);
            }
            else if(player.GetFollowedObject().tag == "Planet") // REACHED A PLANET (FOLLOW CLICKPOINT): change to ShopState
            {
                GameObject planet = player.GetFollowedObject();
                ShopStock currentStock = planet.GetComponent<ShopStock>();
                ShopBarManager.ShopManager.SetCurrentShopStockPointer(currentStock);

                ChangeState(GameState.ShopState);
            }
            else if(player.GetFollowedObject().tag == "Portal")
            {
                Debug.Log("portal");
                ChangeState(GameState.SpaceSystemLoad);
            }
            else // REACHED NON-OBJECT DESTINATION (DEFAULT CLICKPOINT) OR NON-PLANET OBJECT (FOLLOW CLICKPOINT): return to PlanMovement state
            {
                ChangeState(GameState.PlanMovement);
            }
            /*else if(player.GetFollowedObject() == null) 
            {
                ChangeState(GameState.PlanMovement);
            }*/
        }
    }
/*
    void TakeDuringMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ChangeState(GameState.PlanMovement); // stop game
        // else if()
    }

    void FinalStateChecker()
    {
        if(player.GetTarget() == (Vector2) player.transform.position && gameInstance.GetCurrentState() == GameState.DuringMovement)
            ChangeState(GameState.PlanMovement); // stop game
    }
*/
}
