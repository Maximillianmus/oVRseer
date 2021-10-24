using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthorityCollider : MonoBehaviour
{

    public VrInformer informer;

    private void OnCollisionStay(Collision collision)
    {
        if (informer.released == true)
        {
            informer.Release();
        }
    }
}
