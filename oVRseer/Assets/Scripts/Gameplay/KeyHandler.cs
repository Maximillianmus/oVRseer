using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class KeyHandler : MonoBehaviour
{
    public int numberOfKeys = 4;
    private bool doorIsLocked;
    private float textFadeTime = 1.5f;
    public Text keyCollectedMessage;
    public bool textFadeIn;
    private int keysRemaining;


    // Start is called before the first frame update
    void Start()
    {
        keysRemaining = numberOfKeys;
        textFadeIn = false;
        doorIsLocked = true;
        keyCollectedMessage.canvasRenderer.SetAlpha(0);
        
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    private void FadeOutText()
    {
        textFadeIn = false;
    }

    public void KeyCollected()
    {
        keysRemaining -= 1;
        keyCollectedMessage.text = "A key has been collected" + "\n" + keysRemaining + " keys remaining";

        if(keysRemaining <= 0)
        {
            doorIsLocked = false;
        }
    }
}
