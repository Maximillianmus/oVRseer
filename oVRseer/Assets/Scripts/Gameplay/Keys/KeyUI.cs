using System.Collections;
using System.Collections.Generic;
using Mirror;
using Network;
using UnityEngine;
using UnityEngine.UI;

public class KeyUI : MonoBehaviour
{
    public PlayerType type;
    public List<GameObject> clientKeys = new List<GameObject>();
    public int keysRemaining;
    public float textFadeTime = 1.5f;

    public bool textFadeIn = false;

    public GameObject canvas;
    public Text keyCollectedMessage;
    public GameObject door;

    private void Start()
    {
        Font arial;
        arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

        CheckUI();

        // Create the Text GameObject.
        GameObject updateText = new GameObject("UpdateText");
        updateText.transform.parent = canvas.transform;
        updateText.AddComponent<Text>();

        // Set rect properties.
        updateText.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        updateText.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width - 20, 50);

        // Set Text component properties.
        keyCollectedMessage = updateText.GetComponent<Text>();
        keyCollectedMessage.font = arial;
        keyCollectedMessage.fontSize = 15;
        keyCollectedMessage.alignment = TextAnchor.MiddleCenter;
        keyCollectedMessage.text = "";
        keyCollectedMessage.canvasRenderer.SetAlpha(0);

        CheckKeys();
        keysRemaining = clientKeys.Count;
    }

    private void CheckUI()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("IsPlayerUI"))
        {
            if (type == PlayerType.Tiny)
            {
                if (g.name == "OverseerUI")
                {
                    g.SetActive(false);
                }
                else if (g.name == "TinyUI")
                {
                    canvas = g;
                }
            }

            else
            {
                if (g.name == "TinyUI")
                {
                    g.SetActive(false);
                }
                else if (g.name == "OverseerUI")
                {
                    canvas = g;
                }
            }
        }

        door = GameObject.FindGameObjectsWithTag("IsTheDoor")[0];
    }

    private void CheckKeys()
    {
        foreach (GameObject k in GameObject.FindGameObjectsWithTag("IsAKey"))
        {
            clientKeys.Add(k);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (type == PlayerType.Tiny)
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
        keysRemaining -= 1;

        if (keysRemaining > 0)
        {
            keyCollectedMessage.text = "A key has been collected" + "\n" + keysRemaining + " keys remaining";
        }
        else
        {
            keyCollectedMessage.text = "The door is unlocked. Go to it to escape";
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        door.SetActive(false); // Hide for now
    }
}