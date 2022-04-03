using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private Player _player;

    void Start()
    {
        _player = Controller.ControllerGod.player;
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && GameManager.Instance.GetCurrentState() == GameState.PlanMovement)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 dist = mousePos - (Vector2) _player.transform.position;
            Debug.Log("Difference is: " + dist.magnitude);
        }
        
    }
}
