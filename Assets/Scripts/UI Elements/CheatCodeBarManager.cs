using UnityEngine;
using UnityEngine.UI;


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

    public void ConfirmPopup()
    {
        ClosePopup();
        
        cheatCode = inputField.GetComponent<InputField>().text;
        switch(cheatCode)
        {
            case "monet":
                cheatCodeHandler.AddCredit();
                break;
            case "lightning":
                cheatCodeHandler.Speed67();
                break;
            default:
                Debug.Log("Code not valid.");
                break;
        }
        // Debug.Log("YOUR CHEAT CODE IS: " + cheatCode);
        // MORE TO ADD LATER
    }
}
