using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalKey : MonoBehaviour
{
    public GameObject keyHandler;

    // Key is collected
    private void OnTriggerEnter(Collider other)
    {

        // Update UI and number of keys in key handler
        keyHandler.GetComponent<KeyHandler>().FadeInText();
        keyHandler.GetComponent<KeyHandler>().KeyCollected();

        // Delete "key"
        // TODO: Might want to add some cool effect for when this object is collected
        gameObject.SetActive(false);
    }
}