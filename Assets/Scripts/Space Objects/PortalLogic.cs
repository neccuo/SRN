using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalLogic : MonoBehaviour
{
    [SerializeField] private int _boundSystemID; // Maybe check DB for available IDs to error check
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
    }
}
