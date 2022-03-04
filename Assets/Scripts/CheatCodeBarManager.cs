using UnityEngine;
using UnityEngine.UI;


public class CheatCodeBarManager : MonoBehaviour
{

    public GameObject inputField;
    public string cheatCode;

    public CheatCodeHandler cheatCodeHandler;

    private GameState _previousState;

    private GameState _menuState = GameState.MenuState;

    void OnEnable()
    {
        //Debug.Log("PrintOnEnable: script was enabled");
        //Debug.Log(Controller.ControllerGod.ToString());
        _previousState = GameManager.Instance.state;
        Controller.ControllerGod.ChangeState(_menuState);
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
