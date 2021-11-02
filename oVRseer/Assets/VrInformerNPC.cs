using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Experimental;
using UnityEngine;
using NetworkTransformChild = Mirror.NetworkTransformChild;

public class VrInformerNPC : NetworkBehaviour
{
    // Start is called before the first frame update


    public GameObject VrPlayer;
    GrabCommand grabCommand;
    NetworkIdentity networkId;
    Rigidbody Rigidbody;
    public NetworkTransformChild transformPlayer;
    public NetworkRigidbody networkRigidbody;
    public Transform transformArmature;

    [HideInInspector]
    public bool released = false;

    public void Awake()
    {
        VrPlayer = GameObject.FindGameObjectWithTag("VR");
        grabCommand = VrPlayer.GetComponent<GrabCommand>();
        networkId = transform.GetComponent<NetworkIdentity>();
        Rigidbody = gameObject.GetComponentInChildren<Rigidbody>();
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

        transformPlayer.clientAuthority = true;
        networkRigidbody.clientAuthority = true;

        grabCommand.Grab(networkId);
    }


    //tells the vr that this object got released
    //Doesn't work
    public void NotifyVrReleasing()
    {
        if (hasAuthority)
        {
            Invoke("delayedRelease", 0.2f);
        }
    }

    void delayedRelease()
    {
        print("start releasing");
        released = true;
        //used to see if it was an authority or buffer problem  
        //it is a buffer problem, no idea on how to fix
        //Vector3 pos = transformArmature.position;
        //transformPlayer.CmdTeleport(pos);
        //CmdRelease();
    }

    public void Release()
    {
        print("Returned authority");
        released = false;
        Vector3 pos = transformArmature.position;
        transformPlayer.CmdTeleport(pos);
        CmdRelease();
        transformPlayer.clientAuthority = false;
        networkRigidbody.clientAuthority = false;
    }


    [Command]
    public void CmdRelease()
    {
        networkId.RemoveClientAuthority();
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
