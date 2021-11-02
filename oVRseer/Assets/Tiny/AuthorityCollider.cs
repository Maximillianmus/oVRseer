using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class AuthorityCollider : MonoBehaviour
{

    public IVRInformer informer;

    private void OnCollisionStay(Collision collision)
    {
        
        if (informer.isReleased())
        {
            print("releasing");
            informer.Release();
        }
    }
}
