using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mirror;
using Network;

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
        isCollected = true;
    }

    // Key is collected
    private void OnTriggerEnter(Collider other) {
        
        if(other.CompareTag("PlayerArmature")) { 

            isCollected = true;

            if (!isServer) {
                CmdUpdateKeyCollectedToServer();
            }
        }

    }
}
