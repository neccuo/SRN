using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CombatUIMediator : MonoBehaviour
{
    [SerializeField] private Text stateText;
    [SerializeField] private Text phaseText;

    public GameObject cardInfoHolder;


    // public static delegate;

    void Update()
    {
        SetTexts();
    }

    public void SetCardInfo(GameObject card)
    {
        GameObject c = Instantiate(card);
        card.transform.parent = cardInfoHolder.transform;
    }

    public void EmptyCardInfo()
    {
        foreach(Transform child in cardInfoHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // TODO: USE EVENTS FOR OPTIMIZATION
    private void SetTexts()
    {
        CombatState currentState = CombatManager.Instance.GetState();
        CombatPhase currentPhase = CombatManager.Instance.GetPhase();
        
        stateText.text = $"Current State: {currentState}";
        stateText.color = ColorByState(currentState);

        phaseText.text = $"Current Phase: {currentPhase}";
        phaseText.color = ColorByPhase(currentPhase);
    }

    private Color ColorByState(CombatState state)
    {
        switch(state)
        {
            case CombatState.Intermission:
                return Color.gray;
            case CombatState.PlayerTurn:
                return Color.green;
            case CombatState.EnemyTurn:
                return Color.red;
            default:
                return Color.white;
        }
    }

    private Color ColorByPhase(CombatPhase state)
    {
        switch(state)
        {
            case CombatPhase.NullPhase:
                return Color.gray;
            case CombatPhase.Preparation:
                return Color.blue;
            case CombatPhase.Action:
                return Color.red;
            case CombatPhase.After:
                return Color.green;
            default:
                return Color.white;
        }
    }
}
