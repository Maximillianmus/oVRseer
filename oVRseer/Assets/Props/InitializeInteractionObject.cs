using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;



//this component has the objective of giving the authority of this object to the VR player and then turning itself off
public class InitializeInteractionObject : NetworkBehaviour
{
    bool foundVR = false;
    GameObject VrPlayer;

    // Update is called once per frame
    void Update()
    {
        
        VrPlayer = GameObject.FindGameObjectWithTag("VR");

        if (VrPlayer != null){
            CmdGiveAuthority();
            foundVR = true;
        }

        if (foundVR)
        {
            this.enabled = false;
        }

    }

    [Command(requiresAuthority = false)]
    void CmdGiveAuthority()
    {
        print("authority changed");
        netIdentity.RemoveClientAuthority();
        netIdentity.AssignClientAuthority(VrPlayer.GetComponent<NetworkIdentity>().connectionToClient);
    }
}
