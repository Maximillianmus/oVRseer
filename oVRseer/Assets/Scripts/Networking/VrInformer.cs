using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrInformer : MonoBehaviour
{
    // Start is called before the first frame update


    public GameObject VrPlayer;
    GrabCommand grabCommand;
    NetworkIdentity networkId;
    NetworkConnectionToClient owner;

    public void awake()
    {
        VrPlayer = GameObject.FindGameObjectWithTag("VR");
        grabCommand = VrPlayer.GetComponent<GrabCommand>();
        networkId = transform.GetComponent<NetworkIdentity>();
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
           

        grabCommand.Grab(networkId);
    }


    //tells the vr that this object got grabbed
    public void NotifyVrReleasing()
    {
        grabCommand.Release(owner, networkId);
    }


}
