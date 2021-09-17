using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class checkLocalPlayer : MonoBehaviour
{
    public Transform[] disabledTransforms;
    public Component[] disabledComponents;
    



    // Start is called before the first frame update
    void Start()
    {
        if(gameObject != NetworkClient.localPlayer.gameObject)
        {
            for (int i = 0; i < disabledTransforms.Length; i++)
            {
                Transform.Destroy(disabledTransforms[i].gameObject);
            }

            for (int i = 0; i < disabledTransforms.Length; i++)
            {
                print(disabledComponents.Length);
                print(i);
                Component.Destroy(disabledComponents[i]);
            }


        }

    }

}
