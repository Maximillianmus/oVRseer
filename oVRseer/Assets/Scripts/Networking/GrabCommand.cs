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


    public void Grab( NetworkIdentity grabbedNetId)
    {
        CmdGrab(grabbedNetId);
    }

    //this function tells the server that a player has been grabbed
    [Command]
    void CmdGrab(NetworkIdentity grabbed)
    {
        grabbed.RemoveClientAuthority();
        grabbed.AssignClientAuthority(VrIdentity.connectionToClient);
    }


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

}
