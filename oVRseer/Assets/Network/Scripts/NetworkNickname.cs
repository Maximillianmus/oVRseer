using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkNickname : NetworkBehaviour
{
    [SyncVar(hook = "OnChangeNickname")]
    public string nickname = "Player";

    public void OnChangeNickname(string oldNick, string newNick)
    {
        // TODO
        return;
    }
}
