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
        objective = Objective.Patrol; // starts with patrolling for testing

        _currentLoc = (Vector2) transform.position;
        _target = _currentLoc; // exceptional situation, just for initialization


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
                break;

            case Objective.Patrol:
                HandlePatrol();
                break;
        }
    }

    void HandlePatrol()
    {
        if(_currentLoc == _target)
        {
            SetTarget(directions[index%(directions.Length)]);
            index++;
        }
        HandleMovement();
    }

    public Vector2 SetTarget(Vector2 direction)
    {
        _target = _currentLoc + (direction.normalized * patrolRange);
        Debug.Log("Target: " + _target);
        return _target;
    }

    public Vector2 HandleMovement()
    {
        transform.position = Vector2.MoveTowards(_currentLoc, _target, Time.deltaTime * ship.speed);
        return transform.position;
    }
}
