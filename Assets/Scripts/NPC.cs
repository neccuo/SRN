using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Objective
{
    Follow,
    Patrol
}

public class NPC : MonoBehaviour
{
    public Objective objective;
    public Spaceship ship;

    public GameObject player;

    public float patrolRange = 10f;

    private Vector2 _currentLoc;
    private Vector2 _target;
    private Vector2 _dir;
    private float _angle;

    private Vector2[] directions;
    private int index;
    private GameManager currentGameInstance;

    void Start()
    {
        // objective = Objective.Patrol; // starts with patrolling for testing

        _currentLoc = (Vector2) transform.position;
        _target = _currentLoc; // exceptional situation, just for initialization

        if(player == null)
        {
            Debug.Log("Finding Player");
            player = GameObject.Find("Player");
        }

        directions = new Vector2[4]{ Vector2.left, Vector2.up, Vector2.right, Vector2.down};
        index = 0;
        
    }

    void Update()
    {
        _currentLoc = (Vector2) transform.position;
        HandleObjective();
    }

    void HandleObjective()
    {
        switch(objective)
        {
            case Objective.Follow:
                HandleFollow();
                break;

            case Objective.Patrol:
                HandlePatrol();
                break;
        }
    }

    void HandleFollow()
    {
        _target = (Vector2) player.transform.position;
        HandleMovement();
    }

    void HandlePatrol()
    {
        if(_currentLoc == _target)
        {
            SetPatrolTarget(directions[index%(directions.Length)]);
            index++;
        }
        HandleMovement();
    }

    public Vector2 SetPatrolTarget(Vector2 direction)
    {
        _target = _currentLoc + (direction.normalized * patrolRange); // (Vector2) player.transform.position; 
        Debug.Log("Target: " + _target);
        return _target;
    }

    public Vector2 HandleMovement()
    {
        transform.position = Vector2.MoveTowards(_currentLoc, _target, Time.deltaTime * ship.speed);
        _dir = _target - _currentLoc;
        _angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(_angle - 90, Vector3.forward);
        return transform.position;
    }
}
