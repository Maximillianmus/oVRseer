using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "PlayerArmature")
        {
            if (!other.transform.root.GetComponent<NetworkIdentity>().hasAuthority)
                return;
            other.transform.root.GetComponent<State>().Squashed();
        }
    }
}
