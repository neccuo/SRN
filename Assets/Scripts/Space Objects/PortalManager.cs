using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PortalManager : MonoBehaviour
{
    [SerializeField] private SystemManager _systemManager;

    // TODO: GENERALIZE THIS FUNCTION. IT WORKS ONLY FOR PORTALS NOW, PLAN FOR SAVE/LOAD OR LANDING TOO

    public void TravelToSystemByID(int id)
    {
        _systemManager.ChangeSystem(id);

        GameObject player = GameObject.Find("Player");
        Vector3 pos = player.transform.position;

        player.transform.position = new Vector3(-pos.x, -pos.y, pos.z);

        Controller.ControllerGod.ChangeState(GameState.PlanMovement);
    }
    public void CreateSystemToTravel()
    {
        Debug.Log("we are going to be rich!!!");

        //_DestroyChildren(GameObject.Find("Planets"));
        //_DestroyChildren(GameObject.Find("NPCs"));

        GameObject player = GameObject.Find("Player");
        Vector3 pos = player.transform.position;

        player.transform.position = new Vector3(-pos.x, -pos.y, pos.z);

        Controller.ControllerGod.ChangeState(GameState.PlanMovement);
    }

    private void _DisableUnusedChildren(GameObject gObject)
    {
        // TODO: WITH SimpleDB, get
    }

    private void _DestroyChildren(GameObject gobject) // DESTORYS CHILDREN BUT NOT THE PARENT
    {
        foreach(Transform child in gobject.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
