using Mirror;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrInformer : NetworkBehaviour
{
    // Start is called before the first frame update


    public GameObject VrPlayer;
    GrabCommand grabCommand;
    NetworkIdentity networkId;
    NetworkConnectionToClient owner;
    

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
    //Doesn't work
    public void NotifyVrReleasing()
    {
        print(owner);
        print(networkId);
        if (hasAuthority)
        {
            Invoke("CmdRelease", 20);
        }
    }

    [Command]
    void CmdRelease()
    {
        networkId.RemoveClientAuthority();
        networkId.AssignClientAuthority(owner);
    }


}
