using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ship", menuName = "Ship")]
public class Ship : ScriptableObject
{
    
    public string hullName;

    public Sprite hullSprite;
    public SpriteRenderer shipSr;

    public float attack;
    public float health;
    public float shield;

    public void ReceiveDamage(float damageAmount)
    {
        shipSr.color = Color.red;
        float takenDamage = damageAmount - shield;
        if(0 < takenDamage)
        {
            shield = 0;
            Debug.Log($"{hullName} absorbed {shield} damage");

        }
        else
        {
            shield -= damageAmount;
            Debug.Log($"{hullName} absorbed {damageAmount} damage");
        }
        health -= takenDamage;
        CheckDeath();
    }

    public void CheckDeath()
    {
        if(health <= 0)
        {
            health = 0;
            Debug.Log($"{hullName}: Öldün, çýk");
        }
    }
}
