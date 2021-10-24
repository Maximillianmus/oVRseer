using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthorityCollider : MonoBehaviour
{

    public VrInformer informer;

    private void OnCollisionStay(Collision collision)
    {
        print("i collided");
        if (informer.released == true)
        {
            print("Returned authority");
            informer.released = false;
            Vector3 pos = informer.transformArmature.position;
            informer.transformPlayer.CmdTeleport(pos);
            informer.CmdRelease();
        }
    }
}
