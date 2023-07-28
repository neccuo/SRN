using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalLogic : MonoBehaviour
{
    [SerializeField] private int _boundSystemID; // Maybe check DB for available IDs to error check
    [SerializeField] private PortalManager _portalManager; // Maybe check DB for available IDs to error check

    void Awake()
    {
        _portalManager = transform.parent.gameObject.GetComponent<PortalManager>();
    }

    public int GetBoundSystemID()
    {
        return _boundSystemID;
    }

    public void PortalTravelInit() // this one is the first impulse
    {
        _portalManager.TravelToSystemByID(_boundSystemID);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        GameObject collidedObject = col.gameObject;
        Debug.Log("" + collidedObject.name + " collided with " + this.name);
        if(collidedObject.tag == "NPC" && collidedObject.GetComponent<NPC>().teleportReady)
        {
            // MAKE SURE TO ADD SOMETHING USEFUL HERE
            Destroy(collidedObject);
            // ALSO, MAKE SURE TO DEACTIVATE THE TELEPORT PERMISSION SWITCH
        }
        else if(collidedObject.tag == "Player" && collidedObject.GetComponent<Player>().GetFollowedObject() == this.gameObject)
        {
            // <<<DELETE HERE>>>
            Debug.Log("I'm here yo");
            // _portalManager.TravelToSystemByID(_boundSystemID);
        }
    }
}
