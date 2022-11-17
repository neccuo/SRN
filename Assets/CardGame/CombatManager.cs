using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatPhase
{
    NullPhase,
    Preparation,
    Action,
    After
}

public enum CombatState
{
    Intermission,
    PlayerTurn,
    EnemyTurn
}

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    private CombatPhase _phase;
    private CombatState _state;

    [SerializeField] private Ship player;
    [SerializeField] private Ship enemy;

    private ShipSchema playerShip;
    private ShipSchema enemyShip;

    private List<CombatState> stateList;
    int temp;

    void Awake()
    {
        Instance = this;
        _phase = CombatPhase.NullPhase;
        _state = CombatState.Intermission;

        temp = 0;
        stateList = new List<CombatState>
        {
            CombatState.Intermission,
            CombatState.PlayerTurn,
            CombatState.EnemyTurn,
            CombatState.Intermission,
            CombatState.PlayerTurn,
            CombatState.EnemyTurn,
            CombatState.Intermission,
            CombatState.PlayerTurn,
            CombatState.EnemyTurn,
            CombatState.Intermission
        };

        // playerShip = player.ship;
        // enemyShip = enemy.ship;
    }
    void Start()
    {
        // ChangeState(CombatState.PlayerTurn);
    }

    void Update()
    {
        // HandlePhase();
        HandleState();
    }

    public void ChangeState(CombatState newState)
    {
        if(_state == newState)
        {
            return;
        }
        CombatState oldState = _state;
        Debug.Log($"From [{oldState}] to [{newState}]");
        _state = newState;

    }

    // TODO
    //public void ChangePhase(CombatPhase newPhase)

    public CombatState GetState()
    {
        return _state;
    }

    public CombatPhase GetPhase()
    {
        return _phase;
    }

    private void HandleTurn(string turn)
    {
        switch(_phase)
        {
            case CombatPhase.Preparation:
                break;
            case CombatPhase.Action:
                break;
            case CombatPhase.After:
                break;

        }
    }

    private void HandlePlayerTurn()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            float dmg = player.GetDamage();
            Debug.Log("5 vurdun he");
            enemy.ReceiveDamage(dmg);
            // ChangeState(CombatState.EnemyTurn);
            // NextState();
        }

    }
    private void HandleEnemyTurn()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            float dmg = enemy.GetDamage();
            Debug.Log("5 vurdular sana he");
            player.ReceiveDamage(dmg);
            // ChangeState(CombatState.PlayerTurn);
            // NextState();
        }
    }

    private void HandleIntermission()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // NextState();
        }
    }

    public void EndTurn()
    {
        NextState();
    }

    private void HandleState()
    {
        switch(_state)
        {
            case CombatState.Intermission:
                HandleIntermission();
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

    // TODO
    // private void HandlePhase()

    // TEMPORARY ENDEAVOUR
    private void NextState()
    {
        if(temp >= stateList.Count)
        {
            ChangeState(CombatState.Intermission);
            return;
        }
        CombatState cs = stateList[temp];
        temp++;
        ChangeState(cs);
        if(cs == CombatState.Intermission)
        {
            NextPhase();
        }
    }

    private void NextPhase()
    {
        Debug.Log("NextPhase");
        CombatPhase oldPhase = _phase;
        switch(_phase)
        {
            case CombatPhase.NullPhase:
                _phase = CombatPhase.Preparation;
                break;
            case CombatPhase.Preparation:
                _phase = CombatPhase.Action;
                break;
            case CombatPhase.Action:
                _phase = CombatPhase.After;
                break;
            case CombatPhase.After:
                _phase = CombatPhase.Preparation;
                break;
            default:
                throw new System.Exception($"Phase [{_phase}] is not an available phase to handle.");
        }
        Debug.Log($"From [{oldPhase}] to [{_phase}]");
    }
}
