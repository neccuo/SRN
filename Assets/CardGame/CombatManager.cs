using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatState
{
    TurnPrep,
    PlayerTurn,
    EnemyTurn
}

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    private CombatState _state;

    [SerializeField] private Pilot player;
    [SerializeField] private Pilot enemy;

    private Ship playerShip;
    private Ship enemyShip;

    void Awake()
    {
        Instance = this;
        _state = CombatState.TurnPrep;

        playerShip = player.ship;
        enemyShip = enemy.ship;
    }
    void Start()
    {
        ChangeState(CombatState.PlayerTurn);
    }

    void Update()
    {
        HandleState();
    }

    public void ChangeState(CombatState newState)
    {
        if(_state == newState)
        {
            return;
        }
        CombatState oldState = _state;
        // Debug.Log($"From [{oldState}] to [{newState}]");
        _state = newState;

    }

    public CombatState GetState()
    {
        return _state;
    }

    private void HandlePlayerTurn()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float dmg = playerShip.attack;
            Debug.Log("5 vurdun he");
            enemyShip.ReceiveDamage(dmg);
            ChangeState(CombatState.EnemyTurn);
        }

    }
    private void HandleEnemyTurn()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float dmg = enemyShip.attack;
            Debug.Log("5 vurdular sana he");
            playerShip.ReceiveDamage(dmg);
            ChangeState(CombatState.PlayerTurn);
        }

    }

    private void HandleState()
    {
        switch(_state)
        {
            case CombatState.TurnPrep:
                break;
            case CombatState.PlayerTurn:
                HandlePlayerTurn();
                break;
            case CombatState.EnemyTurn:
                HandleEnemyTurn();
                break;
            default:
                throw new System.Exception($"State [{_state}] is not an available state to handle.");
        }
    }
}
