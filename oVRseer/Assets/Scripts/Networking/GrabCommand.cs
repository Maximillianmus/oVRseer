using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;



//this class handles grabing for networking purposes
public class GrabCommand : NetworkBehaviour
{

    XRGrabInteractable grabHand;
    Transform CurrentlyHeldObject;



    private void Start()
    {
        grabHand = gameObject.GetComponent<XRGrabInteractable>();
    }


    void Grab()
    {
       


    }

    //this function tells the server that a player has been grabbed
    [Command]
    void CmdGrab(NetworkIdentity netId)
    {

    }


    void Release()
    {

    }
    [Command]
    void CmdRelease(NetworkIdentity netId)
    {

    }

}
