using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NullHandlers : NetworkBehaviour
{
    public override void OnStartAuthority()
    {
        NetworkClient.RegisterHandler<KeyCollectedMsg>(DeadFunction);
    }

    private void DeadFunction(KeyCollectedMsg msg)
    {
    }
}
