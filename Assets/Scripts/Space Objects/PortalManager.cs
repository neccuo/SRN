using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PortalManager : MonoBehaviour
{
    public void CreateSystemToTravel()
    {
        Debug.Log("we are going to be rich!!!");

        // SHITTIEST HACK, DEFINITELY WILL CHANGE IT
        /*DontDestroyOnLoad(GameObject.Find("Game Manager"));
        DontDestroyOnLoad(GameObject.Find("ClickPoint"));
        DontDestroyOnLoad(GameObject.Find("Main Camera"));
        DontDestroyOnLoad(GameObject.Find("main_space"));
        DontDestroyOnLoad(GameObject.Find("Player"));
        DontDestroyOnLoad(GameObject.Find("Canvas"));
        DontDestroyOnLoad(GameObject.Find("EventSystem"));

        SceneManager.CreateScene("asfasdgasdg");
        SceneManager.LoadScene("asfasdgasdg");*/

        Destroy(GameObject.Find("Planets"));
        Destroy(GameObject.Find("Sun"));
        Destroy(GameObject.Find("NPC"));

        GameObject player = GameObject.Find("Player");
        Vector3 pos = player.transform.position;

        player.transform.position = new Vector3(-pos.x, -pos.y, pos.z);

        Controller.ControllerGod.ChangeState(GameState.PlanMovement);
    }
}
