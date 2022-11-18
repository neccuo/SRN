using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Standard,
    Shield,
    Attack,
    Recovery
}


[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public string description;

    public Sprite artwork;
    public CardType cardType = CardType.Standard;
    public int manaCost;
    public int attack;
    public int health;
}
