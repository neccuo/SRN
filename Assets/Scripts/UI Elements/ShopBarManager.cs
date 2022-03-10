using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopBarManager : MonoBehaviour
{

    void OnEnable()
    {
        Debug.Log("Shop Bar is Enabled");
    }

    void OnDisable()
    {
        Debug.Log("Shop Bar is Disabled");
        Controller.ControllerGod.ChangeState(GameState.PlanMovement);
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}