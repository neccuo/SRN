using UnityEngine.UI;
using UnityEngine;


// NOTES TO SELF
// DUE TO THE SOME CHANGES I MADE, THIS CLASS ONLY WORKS AS A MEDIATOR TO UPDATE THE CREDITS TEXT UI
// THIS MEANS AT SOME POINT I MIGHT THINK OF DELETING THIS CLASS
public class CreditManager : MonoBehaviour
{
    public Text creditsText;
    private int _credits;

    private SystemDB _systemDB;

    void Start()
    {
        _systemDB = gameObject.GetComponent<GameManager>().SYSTEMDB;
        UpdateCredits();
    }

    // Works as a Tick
    public void UpdateCredits()
    {
        _credits = _systemDB.GetPlayerCredits();
        UpdateText();
    }

    void UpdateText()
    {
        creditsText.text = "CREDITS: " + _credits;
    }
}
