using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatCodeHandler : MonoBehaviour
{
    public CreditManager creditManager;

    public Player player;

    public void AddCredit()
    {
        creditManager.ExchangeCredits(1000);
    }

    public void Speed67()
    {
        player.ship.speed = 67;
    }
}
