using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ship Schema", menuName = "ShipSchema")]
public class ShipSchema : ScriptableObject
{
    public string hullName;

    public Sprite hullSprite;

    public float baseAttack;
    public float baseHealth;
    public float baseShield;
    public float baseRecovery;


}
