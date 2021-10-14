using System;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;



/*
 * sript for despawning the player incase of death or victory
 */
public class Despawn : NetworkBehaviour
{
    public Canvas textCanvas;
    private Transform textTransform;
    private Text textComponent;
    private Transform textBackground;

    //winning
    public string winText;
    public Color winColor;
    public Font winFont;


    //dying
    public string deathText;
    public Color deathColor;
    public Font deathFont;

    private float TimeDead;
    public float delayForSpectating;
    public UnityEvent spectatingEvent;
    private bool Dead = false;
    private bool alreadySpectating = false;




    public void Start()
    {
        textTransform = textCanvas.transform.Find("EndText");
        textComponent = textTransform.GetComponent<Text>();
        textBackground = textCanvas.transform.Find("EndBackground");
        textComponent.text = "";
    }

    private void Update()
    {
        if (alreadySpectating)
        {
            return;
        }
        if (Dead && Time.time - TimeDead > delayForSpectating)
        {
            textBackground.gameObject.SetActive(false);
            textComponent.gameObject.SetActive(false);
            alreadySpectating = true;
            spectatingEvent.Invoke();
        }
        
    }


    //spectating should be activated in both of these

    //This player has won
    public void Win()
    {

        textComponent.text = winText;
        textComponent.color = winColor;
        textComponent.font = winFont;
        EnableText();
        //TODO particle effect when winning
        TimeDead = Time.time;
        Dead = true;
    }


    //this player had died
    public void Kill()
    {
        textComponent.text = deathText;
        textComponent.color = deathColor;
        textComponent.font = deathFont;
        EnableText();
        //TODO particle effect when dead
        TimeDead = Time.time;
        Dead = true;
    }

    public void EnableText()
    {
        textBackground.gameObject.SetActive(true);
    }

    //can be used to remove overlay and text when spectating
    public void DisableText()
    {
        textTransform.gameObject.SetActive(false);
        textBackground.gameObject.SetActive(false);
    }
    
    


}
