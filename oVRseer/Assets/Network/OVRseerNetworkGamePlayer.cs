using System.Collections;
using System.Collections.Generic;
using Mirror;
using Network;
using UnityEngine;

public class OVRseerNetworkGamePlayer : NetworkBehaviour
{
    
    private OVRseerNetworkManager room;
    private OVRseerNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as OVRseerNetworkManager;
        }
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);

    }
 
    
}
