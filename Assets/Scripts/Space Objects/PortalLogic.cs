using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalLogic : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        GameObject collidedObject = col.gameObject;
        if(collidedObject.tag == "NPC" && collidedObject.GetComponent<NPC>().teleportReady)
        {
            Debug.Log("You can teleport");
            // MAKE SURE TO ADD SOMETHING USEFUL HERE
            Destroy(collidedObject);
            // ALSO, MAKE SURE TO DEACTIVATE THE TELEPORT PERMISSION SWITCH
        }
    }
}
