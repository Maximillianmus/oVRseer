using Mirror;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrInformer : NetworkBehaviour
{
    // Start is called before the first frame update


    public GameObject VrPlayer;
    public bool forceAuthority = false;
    GrabCommand grabCommand;
    NetworkIdentity networkId;
    NetworkConnectionToClient owner;
    public RigidbodyThirdPersonController rigidbodyThirdpersonController;
    

    public void Awake()
    {
        VrPlayer = GameObject.FindGameObjectWithTag("VR");
        grabCommand = VrPlayer.GetComponent<GrabCommand>();
        networkId = transform.GetComponent<NetworkIdentity>();
        owner = networkId.connectionToClient;
    }

    //tells the vr that this object got grabbed
    public void NotifyVrGrabbing()
    {
        // incase the vr player had yet to spawn in when this object was made 
        if(VrPlayer == null)
        {
            VrPlayer = GameObject.FindGameObjectWithTag("VR");
            grabCommand = VrPlayer.GetComponent<GrabCommand>();
        }

        owner = networkId.connectionToClient;
        print("----networkId---");
        print(networkId.netId);
        print("-----connection.....");
        print(owner);
        grabCommand.Grab(networkId);
    }


    //tells the vr that this object got released
    public void NotifyVrReleasing()
    {
        print(owner);
        print(networkId);

        //grabCommand.Release(owner, networkId);
        while(!forceAuthority && !rigidbodyThirdpersonController.Grounded)
        {

        }
        print("we are grounded");
        if (hasAuthority)
        {
            CmdRelease();
        }
    }

    [Command]
    void CmdRelease()
    {
        networkId.RemoveClientAuthority();
        networkId.AssignClientAuthority(owner);
    }


}
