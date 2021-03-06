using System.Collections;
using System.Collections.Generic;
using Mirror;
using Network;
using UnityEngine;
using UnityEngine.UI;

public class KeyUI : NetworkBehaviour
{
    public bool isVrPlayer;
    public List<GameObject> clientKeys = new List<GameObject>();
    public int keysRemainingTotal;
    public int keysRemaingToOpenDors;
    public float textFadeTime = 1.5f;

    public bool textFadeIn = false;

    public GameObject canvas;
    public Text keyCollectedMessage;
    public GameObject[] doors;
    public GameObject[] doorLights;

    private void Start()
    {
        isVrPlayer = gameObject.GetComponent<checkLocalPlayer>().isVr;

        if (!isVrPlayer)
        {
            CreateInfoText();

            doors = GameObject.FindGameObjectsWithTag("IsTheDoor");
            doorLights = GameObject.FindGameObjectsWithTag("DoorLight");
            HideDoorLights();
            CheckKeys();
            keysRemainingTotal = clientKeys.Count;
            keysRemaingToOpenDors = (keysRemainingTotal / 2);

        }
    }

    private void CreateInfoText()
    {
        // Get the font
        Font arial;
        arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

        // Create the Text GameObject.
        GameObject updateText = new GameObject("UpdateText");
        updateText.transform.parent = canvas.transform;
        updateText.AddComponent<Text>();

        // Set rect properties.
        updateText.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        updateText.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        updateText.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, 100);

        // Set Text component properties.
        keyCollectedMessage = updateText.GetComponent<Text>();
        keyCollectedMessage.font = arial;
        keyCollectedMessage.fontSize = 40;
        keyCollectedMessage.alignment = TextAnchor.MiddleCenter;
        keyCollectedMessage.text = "";
        keyCollectedMessage.canvasRenderer.SetAlpha(0);
    }

    private void CheckKeys()
    {
        foreach (GameObject k in GameObject.FindGameObjectsWithTag("IsAKey")) 
        {
            clientKeys.Add(k);
        }
    }

    private void HideDoorLights()
    {
        foreach(GameObject dl in doorLights)
        {
            dl.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!isVrPlayer)
        {
            foreach (GameObject key in clientKeys)
            {
                if (key.GetComponent<Key>().isCollected)
                {
                    KeyCollected();
                    FadeInText();
                    clientKeys.Remove(key);
                    break;
                }
            }

            TextFade();

        }

    }

    private void TextFade()
    {
        if (textFadeIn == true)
        {
            //Fully fade in text with duration of "textFadeTime" 
            keyCollectedMessage.CrossFadeAlpha(1, textFadeTime, false);
            Invoke("FadeOutText", textFadeTime);

        }

        if (textFadeIn == false)
        {
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
        keysRemainingTotal -= 1;

        if (keysRemaingToOpenDors > 0) { 

        keysRemaingToOpenDors -= 1;

            if (keysRemaingToOpenDors > 0)
            {
                keyCollectedMessage.text = "A key has been collected!" + "\nCollect " + keysRemaingToOpenDors + " more keys to unlock the doors";
            }
            else
            {
                keyCollectedMessage.text = "The doors are unlocked. Go to either of them to escape";
                OpenDoor();
            }
        }
    }

    private void OpenDoor()
    {
        Animator doorLightAnimator;

        foreach (GameObject dl in doorLights)
        {
            dl.SetActive(true);
            doorLightAnimator = dl.transform.Find("LightBeamCylinder").GetComponent<Animator>();
            doorLightAnimator.Play("DoorLightRise");
            //dl.transform.Find("DoorLightParticles");
        }

        Animator doorAnimator;

        foreach (GameObject door in doors) {
            //door.SetActive(false); // Hide doors for now
            doorAnimator = door.GetComponent<Animator>();
            doorAnimator.Play("UnlockDoors");
            //Destroy(door.GetComponent<BoxCollider>()); // Destroy to be able to actually get out
        }
    }
}