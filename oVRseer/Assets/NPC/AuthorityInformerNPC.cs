using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthorityInformerNPC: MonoBehaviour
{
    public VrInformerNPC informer;

    private void OnCollisionStay(Collision collision)
    {

        if (informer.released)
        {
            informer.Release();
        }
    }
}