using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UIElements;

public class Ship : MonoBehaviour
{
    public ShipSchema ss;
    private SpriteRenderer sRenderer;

    private string _hullName;

    private float _attack;
    private float _health;
    private float _shield;
    private float _recovery;

    private float _maxHealth;



    public void Awake()
    {
        _hullName = ss.hullName;

        _attack = ss.baseAttack;
        _health = ss.baseHealth;
        _maxHealth = ss.baseAttack;
        _shield = ss.baseShield;
        _recovery = ss.baseRecovery;
    }

    void Start()
    {
        sRenderer = GetComponent<SpriteRenderer>();
        sRenderer.sprite = ss.hullSprite;
    }

    public float GetDamage()
    {
        return _attack;
    }

    public string GetHullName()
    {
        return _hullName;
    }

    public void ReceiveDamage(float damageAmount)
    {
        // sRenderer.color = Color.red;
        float takenDamage = damageAmount - _shield;
        if (0 < takenDamage)
        {
            _shield = 0;
            _health -= takenDamage;
            Debug.Log($"{_hullName} took {takenDamage} (absorbed {_shield} damage)");
        }
        else
        {
            _shield -= damageAmount;
            Debug.Log($"{_hullName} absorbed {damageAmount} damage");
        }
        CheckDeath();
    }

    public void RecoverHealth()
    {
        float finalHealth = _recovery + _health;
        if(finalHealth > _maxHealth)
        {
            finalHealth = _maxHealth;
        }
        _health = finalHealth;
    }

    public void ShieldUp(float addShield)
    {
        _shield = addShield;
    }

    public void CheckDeath()
    {
        if (_health <= 0)
        {
            _health = 0;
            Debug.Log($"{_hullName}: �ld�n, ��k");
        }
    }
}
