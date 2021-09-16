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


    [Command]
    public void CmdGoToLobby()
    {
        
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 120, 40, 120, 20), "Disconnect"))
        {
            CmdGoToLobby();
        } 
    }
}
