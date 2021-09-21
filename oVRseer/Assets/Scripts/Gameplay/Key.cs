using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Key : MonoBehaviour
{
    //public Text keyCollectedMessage;
    // bool textFadeIn = false;

    //private float textFadeTime = 1.5f;
    public GameObject keyHandler;


    private void OnTriggerEnter(Collider other) {
        // Key is collected, TODO: need to update UI for all tiny-guy, send message to keyhandler, delet key. 
        keyHandler.GetComponent<KeyHandler>().textFadeIn = true;
        keyHandler.GetComponent<KeyHandler>().KeyCollected();
    }

    // Start is called before the first frame update
    void Start()
    {
        //keyCollectedMessage.canvasRenderer.SetAlpha(0);
        //keyCollectedMessage.text = "A key has been collected";
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (textFadeIn == true)
        {
            //Fully fade in Image (1) with the duration of 2
            keyCollectedMessage.CrossFadeAlpha(1, textFadeTime, false);
            Invoke("FadeOutText", textFadeTime + 1);
        }

        if (textFadeIn == false)
        {
            keyCollectedMessage.CrossFadeAlpha(0, 1.0f, false);
        }
        */
    }
    /*
    private void FadeOutText()
    {
        textFadeIn = false;
    }
    */
}
