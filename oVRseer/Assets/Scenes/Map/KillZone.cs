using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "PlayerArmature")
        {
            other.transform.root.GetComponent<State>().Squashed();
        }
    }
}
