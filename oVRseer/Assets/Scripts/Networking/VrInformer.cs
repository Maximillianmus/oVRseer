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

        var position = transform.position;
        var rotation = transform.rotation;

        grabCommand.Grab(networkId, position, rotation);
    }


    //tells the vr that this object got released
    //Doesn't work
    public void NotifyVrReleasing()
    {
        print(owner);
        print(networkId);
        if (hasAuthority)
        {
            //used to see if it was an authority or buffer problem  
            //it is a buffer problem, no idea on how to fix
            CmdRelease();
        }
    }

    [Command]
    void CmdRelease()
    {
        Vector3 oldPos = transform.position;
        Quaternion oldRot = transform.rotation;
        networkId.RemoveClientAuthority();
        networkId.AssignClientAuthority(owner);
        RpcNewpos(oldPos, oldRot); 

    }

    [ClientRpc]
    void RpcNewpos( Vector3 oldPos, Quaternion oldRot)
    {
        transform.position = oldPos;
        transform.rotation = oldRot;
    }

    public override void OnStartAuthority()
    {
        if (!VrPlayer.GetComponent<NetworkIdentity>().hasAuthority)
        {
            var rigid = GetComponentInChildren<Rigidbody>();
            rigid.isKinematic = false;
            rigid.useGravity = true;
        }
        base.OnStartAuthority();
        
    }
}
