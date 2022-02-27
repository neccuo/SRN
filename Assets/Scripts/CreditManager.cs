using UnityEngine.UI;
using UnityEngine;

public class CreditManager : MonoBehaviour
{
    public Text creditText;
    private int _credit;

    void Awake()
    {
        /// ***IMPORTANT***
        /// CHANGE "playerMoney" AFTER USING A PROPER DATABASE
        /// SINCE CreditManager IS NOT ONLY FOR THE PLAYER

        _credit = PlayerPrefs.GetInt("playerMoney", 1000); // init credits
        UpdateText();
        Debug.Log("Money is " + _credit);

    }

    public void ExchangeCredits(int amount)
    {
        SetCredits(_credit + amount);
    }

    void SetCredits(int amount)
    {
        _credit = amount;
        PlayerPrefs.SetInt("playerMoney", _credit);
        Debug.Log("Money is changed to " + _credit);
        UpdateText();

    }

    public int GetCredits()
    {
        return _credit;
    }

    void UpdateText()
    {
        creditText.text = "CREDIT: " + GetCredits();
    }
}
