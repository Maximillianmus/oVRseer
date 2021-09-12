using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerN : NetworkBehaviour
{
    void HandleMovement()
    {
        if (isLocalPlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal *0.1f, 0, moveVertical*0.1f);
            transform.position = transform.position + movement;
        }
    }

    private void Update()
    {
        HandleMovement();
    }

}
