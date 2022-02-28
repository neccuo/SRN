using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatCodeHandler : MonoBehaviour
{
    public CreditManager creditManager;

    public void AddCredit()
    {
        creditManager.ExchangeCredits(1000);
    }
}
