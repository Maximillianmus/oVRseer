using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerArmature")
        {
            other.transform.root.GetComponent<State>().Squashed();
        }

        Destroy(other.gameObject);
    }
}
