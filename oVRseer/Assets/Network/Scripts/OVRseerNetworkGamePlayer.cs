using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Network;
using UnityEngine;
using UnityEngine.UI;

public class OVRseerNetworkGamePlayer : NetworkBehaviour
{
    public PlayerType type;
    [SyncVar]
    public string nickname; 
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



    void OnGUI()
    {
        /*
        if (GUI.Button(new Rect(Screen.width - 120, 40, 120, 20), "Disconnect"))
        {
            if (isServer && isClient)
            {
                Room.StopHost();
                return;
            }

            if (isServer)
            {
                Room.StopServer();
                return;
            }
            if (isClient)
            {
                Room.StopClient();
            }
        } 
        */
    }
}
