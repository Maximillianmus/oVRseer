using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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




    public void Start()
    {
        textTransform = textCanvas.transform.Find("EndText");
        textComponent = textTransform.GetComponent<Text>();
        textBackground = textCanvas.transform.Find("EndBackground");
        textComponent.text = "";
    }


    //spectating should be activated in both of these

    //This player has won
    public void Win()
    {

        textComponent.text = winText;
        textComponent.color = winColor;
        EnableText();
        //TODO particle effect when winning
        CmdDespawn();
    }


    //this player had died
    public void Kill()
    {
        textComponent.text = deathText;
        textComponent.color = deathColor;
        EnableText();
        //TODO particle effect when dead
        CmdDespawn();
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

    //depending on how the spectating works this migh have to be changed
    [Command]
    private void CmdDespawn()
    {
        //if the player should be destory, but currently the camera object is inside the player so that doesn't work if we want spectating
        //if we spawn or move the camera when spectating then we can just despawn this object
        //NetworkServer.Destroy(gameObject);
        transform.Find("PlayerArmature").gameObject.SetActive(false);
    }

}
