using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 target;
    private Vector2 dir;
    private float angle;
    private GameManager currentGameInstance;

    void SetMovement()
    {
        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void Start()
    {
        target = transform.position;
        angle = transform.rotation.z;

        currentGameInstance = GameManager.Instance;
    }

    void Update()
    {
        // currentGameInstance = GameManager.Instance; // update the game state

        switch (currentGameInstance.State)
        {
            case GameState.PlanMovement:
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
                {
                    SetMovement();
                }
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    currentGameInstance.UpdateGameState(GameState.DuringMovement);
                }
                break;
            case GameState.DuringMovement:

                transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * 4f);
                if((Vector2)transform.position == target)
                {
                    currentGameInstance.UpdateGameState(GameState.PlanMovement);
                }
                break;
            case GameState.TurnEvaluation:
                break;
            case GameState.Combat:
                break;
            default:
                throw new MissingComponentException("" + currentGameInstance.ToString() + "is not an available state.");
        }

        



        // GetMouseButton(0) is temporary since this game is turn based (maybe)
        //if (input.getmousebuttondown(0) || input.getmousebutton(0))
        //{
        //    target = camera.main.screentoworldpoint(input.mouseposition);
        //    dir = input.mouseposition - camera.main.worldtoscreenpoint(transform.position);
        //    angle = mathf.atan2(dir.y, dir.x) * mathf.rad2deg;
        //    transform.rotation = quaternion.angleaxis(angle - 90, vector3.forward);
        //}
        //if(currentgameinstance.state == gamestate.duringmovement)
        //{
        //    transform.position = vector2.movetowards(transform.position, target, time.deltatime * 4f);
        //}

        //if (currentgameinstance.state == gamestate.duringmovement && (vector2) transform.position == target)
        //{
        //    currentgameinstance.updategamestate(gamestate.planmovement);
        //}


    }
}
