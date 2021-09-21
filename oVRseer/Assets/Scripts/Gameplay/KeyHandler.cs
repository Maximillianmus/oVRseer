using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class KeyHandler : MonoBehaviour
{
    public int numberOfKeys = 1;
    public Text keyCollectedMessage;
    public Vector3[] keyPositions;
    public float textFadeTime = 1.5f;

    private int maxNumberOfKeys = 4;
    private bool textFadeIn;
    private int keysRemaining;

    public GameObject keyPrefab;
    public GameObject theKeyHandler;

    // Start is called before the first frame update
    void Start()
    {
        
        textFadeIn = false;


        if(numberOfKeys > maxNumberOfKeys) {
            numberOfKeys = maxNumberOfKeys;
        }

        keysRemaining = numberOfKeys;
        keyCollectedMessage.canvasRenderer.SetAlpha(0);

        FillPositions();
        CreateKeys();
    }

    // Update is called once per frame
    void Update()
    {
        TextFade();
    }

    private void TextFade()
    {
        if (textFadeIn == true) {
            //Fully fade in text with duration of "textFadeTime" 
            keyCollectedMessage.CrossFadeAlpha(1, textFadeTime, false);
            Invoke("FadeOutText", textFadeTime);

        }

        if (textFadeIn == false){
            keyCollectedMessage.CrossFadeAlpha(0, 1f, false);
        }
    }

    public void FadeOutText()
    {
        textFadeIn = false;
    }

    public void FadeInText()
    {
        textFadeIn = true;
    }

    public void KeyCollected()
    {
        keysRemaining -= 1;

        if(keysRemaining > 0)
        {
            keyCollectedMessage.text = "A key has been collected" + "\n" + keysRemaining + " keys remaining";
        }
        else
        {
            keyCollectedMessage.text = "The door is unlocked. Go to it to escape";
            OpenDoor();
        }
    }

    private void FillPositions()
    {
        // Hard coded positions for now
        keyPositions = new Vector3[4];
        keyPositions[0] = new Vector3(-5f, 5f, -14f);
        keyPositions[1] = new Vector3(-5f, 5f, -3f);
        keyPositions[2] = new Vector3(7.5f, 5f, -14f);
        keyPositions[3] = new Vector3(7.5f, 5f, -3f);
    }

    private void CreateKeys() {

        for(int i = 0; i < numberOfKeys; i++)
        {
            GameObject temp = Instantiate(keyPrefab, keyPositions[i], Quaternion.identity) as GameObject;
            temp.name = "Key" + i.ToString();
            temp.GetComponent<Key>().keyHandler = theKeyHandler;
        }
    }

    private void OpenDoor()
    {
        //TODO: Add code to open 'door' so that tiny-guys can escape
    }
}
