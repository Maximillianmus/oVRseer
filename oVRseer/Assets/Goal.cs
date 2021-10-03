using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.transform.root.GetComponent<Despawn>().Win();
        }
    }
}
