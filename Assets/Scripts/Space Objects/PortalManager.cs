using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PortalManager : MonoBehaviour
{

    // TODO: GENERALIZE THIS FUNCTION. IT WORKS ONLY FOR PORTALS NOW, PLAN FOR SAVE/LOAD OR LANDING TOO
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

        //Destroy(GameObject.Find("Planets"));
        //Destroy(GameObject.Find("NPC"));

        _DestroyChildren(GameObject.Find("Planets"));
        _DestroyChildren(GameObject.Find("NPCs"));

        //Destroy(GameObject.Find("Sun")); // MAYBE CHANGE ANIMATION SET, OCCURENCES


        GameObject player = GameObject.Find("Player");
        Vector3 pos = player.transform.position;

        player.transform.position = new Vector3(-pos.x, -pos.y, pos.z);

        Controller.ControllerGod.ChangeState(GameState.PlanMovement);
    }

    private void _DestroyChildren(GameObject gobject) // DESTORYS CHILDREN BUT NOT THE PARENT
    {
        foreach(Transform child in gobject.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
