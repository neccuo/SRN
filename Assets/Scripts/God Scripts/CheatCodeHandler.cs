using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatCodeHandler : MonoBehaviour
{
    public CreditManager creditManager;

    public Player player;

    private GameManager _gm = GameManager.Instance;

    public void AddCredit()
    {
        creditManager.ExchangeCredits(1000);
    }

    public void Speed67()
    {
        player.ship.speed = 67;
    }

    public void KillAll() // TODO: LOCK OTHER INPUTS
    {
        // ***POTENTIAL THREAT***
        // WHILE DELETING EVERY NPC FROM THE SAVE FILE AND FROM THE GAME, SYSTEM CAN ADD NPCS SYNCRONOUSLY. SO, IDS MAY MIX
        foreach(Transform child in GameObject.Find("NPCs").transform)
        {
            Destroy(child.gameObject);
        }
        PlayerPrefs.SetInt("npcSpawned", 0);
        StartCoroutine(_gm.saveLoad.EraseNPCS());
    }
}
