using UnityEngine;
using UnityEngine.UI;


public class CheatCodeBarManager : MonoBehaviour
{

    public GameObject inputField;
    public string cheatCode;

    public CheatCodeHandler cheatCodeHandler;


    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }

    public void ConfirmPopup()
    {
        cheatCode = inputField.GetComponent<InputField>().text;
        switch(cheatCode)
        {
            case "monet":
                cheatCodeHandler.AddCredit();
                break;
            default:
                Debug.Log("Code not valid.");
                break;
        }
        // Debug.Log("YOUR CHEAT CODE IS: " + cheatCode);
        // MORE TO ADD LATER
    }
}
