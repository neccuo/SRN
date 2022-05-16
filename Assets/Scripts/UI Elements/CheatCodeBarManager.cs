using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


public class CheatCodeBarManager : MonoBehaviour
{

    public GameObject inputField;
    public string cheatCode;

    public CheatCodeHandler cheatCodeHandler;

    private GameState _previousState;

    public GameState officialState = GameState.CheatBarState;

    // private GameState _ourState = GameState.CheatBarState;

    public void SetPreviousState(GameState state)
    {
        if(state == GameState.CheatBarState)
        {
            throw new UnityException("HOW DID YOU CAME FROM CHEATBAR TO CHEATBAR");
        }
        _previousState = state;
    }

    void OnEnable()
    {
        // TODO: CHANGE STATES ACCORDINGLY IF IT IS ENABLED AT THE START OF THE GAME
        if(GameManager.Instance.GetCurrentState() != officialState)
        {
            Controller.ControllerGod.ChangeState(officialState);
        }
        Debug.Log("Cheat Code Bar is Enabled");
    }

    void OnDisable()
    {
        //Debug.Log("PrintOnDisable: script was disabled");
        //Debug.Log(_previousState.ToString());
        Controller.ControllerGod.ChangeState(_previousState);
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
        //Controller.ControllerGod.ChangeState();
    }

    private string _IsPrefixValid(string str)
    {
        // "BUY: "
        string prefix = str.Substring(0, 2);
        if(prefix == "BUY")
        {
            return prefix;
        }
        // else
        return "";
    }

    private List<string> _ParseCommand(string str)
    {
        List<string> strList = str.Split().ToList();

        return strList;
    }

    private bool _CheckListLength(List<string> strLi, int num)
    {
        if(strLi.Count == num)
            return true;
        return false;
    }

    public void ConfirmPopup()
    {
        ClosePopup();
        
        cheatCode = inputField.GetComponent<InputField>().text;
        List<string> strList = _ParseCommand(cheatCode);
        switch(strList[0])
        {
            case "monet":
                cheatCodeHandler.AddCredit();
                break;
            case "lightning":
                cheatCodeHandler.Speed67();
                break;
            case "killall":
                cheatCodeHandler.KillAll();
                break;
            case "BUY":
                if(_CheckListLength(strList, 4))
                    cheatCodeHandler.BuyItemNpc(strList[1], strList[2], strList[3]);
                else
                    Debug.LogWarning("Poorly formatted [" + strList[0] + "] code");
                break;
            default:
                Debug.Log("Code not valid.");
                break;
        }
        // Debug.Log("YOUR CHEAT CODE IS: " + cheatCode);
        // MORE TO ADD LATER
    }
}
