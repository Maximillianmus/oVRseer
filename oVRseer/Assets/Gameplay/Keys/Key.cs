using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mirror;
using Network;

public struct KeyCollectedMsg : NetworkMessage
{
}


public class Key : NetworkBehaviour
{
    public NetworkIdentity keyNetId;
    [SyncVar(hook = nameof(KeyCollected))]
    public bool isCollected = false;
    private bool isHidden = false;

    private void Start()
    {
        keyNetId = transform.GetComponent<NetworkIdentity>();
    }

    private void Update()
    {
        if (isCollected && !isHidden)
        {
            // Debug.Log("Collected the key");
            isHidden = true;
            gameObject.SetActive(false); // Hide key for now 
        }
    }

    void KeyCollected(bool oldValue, bool newValue) {
        isCollected = newValue;
    }

    [Command(requiresAuthority = false)]
    void CmdUpdateKeyCollectedToServer() {
        if (!isCollected)
        {
            isCollected = true;
            NetworkServer.SendToAll(new KeyCollectedMsg());
        }
    }

    // Key is collected
    private void OnTriggerEnter(Collider other) {
        
        if(other.CompareTag("PlayerArmature") && !isCollected) { 

            isCollected = true;

            if (!isServer) {
                CmdUpdateKeyCollectedToServer();
            }
        }

    }
}
