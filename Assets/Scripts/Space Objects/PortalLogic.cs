using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PortalLogic : MonoBehaviour
{
    public void CreateSystemToTravel()
    {
        Debug.LogWarning("we are going to be rich!!!");

        // SHITTIEST HACK, DEFINITELY WILL CHANGE IT
        DontDestroyOnLoad(GameObject.Find("Game Manager"));
        DontDestroyOnLoad(GameObject.Find("ClickPoint"));
        DontDestroyOnLoad(GameObject.Find("Main Camera"));
        DontDestroyOnLoad(GameObject.Find("main_space"));
        DontDestroyOnLoad(GameObject.Find("Player"));
        DontDestroyOnLoad(GameObject.Find("Canvas"));
        DontDestroyOnLoad(GameObject.Find("EventSystem"));

        SceneManager.CreateScene("asfasdgasdg");
        SceneManager.LoadScene("asfasdgasdg");



    }
}
