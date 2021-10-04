using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;



//this class handles grabing for networking purposes
public class GrabCommand : NetworkBehaviour
{
    NetworkIdentity VrIdentity;



    private void Start()
    {
        VrIdentity = transform.GetComponent<NetworkIdentity>();
    }


    public void Grab( NetworkIdentity grabbedNetId, Vector3 position, Quaternion rotation)
    {

        if(hasAuthority)
            CmdGrab(grabbedNetId, position, rotation);
    }

    //this function tells the server that a player has been grabbed
    [Command]
    void CmdGrab(NetworkIdentity grabbed, Vector3 position, Quaternion rotation)
    {
        grabbed.gameObject.transform.position = position;
        grabbed.gameObject.transform.rotation = rotation;
        grabbed.RemoveClientAuthority();
        grabbed.AssignClientAuthority(VrIdentity.connectionToClient);
    }

    /*
    //doesn't work
    public void Release(NetworkConnectionToClient modelOwner, NetworkIdentity grabbedNetId)
    {
        CmdRelease(modelOwner, grabbedNetId);
    }
    [Command]
    void CmdRelease(NetworkConnectionToClient modelOwner,NetworkIdentity grabbed)
    {
        grabbed.RemoveClientAuthority();
        grabbed.AssignClientAuthority(modelOwner);
    }
    */

}
