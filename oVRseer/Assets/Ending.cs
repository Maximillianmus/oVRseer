using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Ending : NetworkBehaviour
{
    public GameObject generalUI;
    public GameObject winning;
    public GameObject losing;
    public Text countText;

    [SerializeField]
    private bool isVrPlayer;


    private void Start()
    {
        if (!hasAuthority)
            return;
        NetworkClient.RegisterHandler<PlayerCount>(OnEnd);
    }

    public void OnEnd(PlayerCount msg)
    {
        print("_______1");
        int outsides = msg.outsides;
        int squasheds = msg.squashed;
        if (!GetComponent<checkLocalPlayer>().assignedAsLocalPlayer)
        {
            return;
        }
        print("_______2");
        countText.text = outsides + " players succeed to escape but " + squasheds + " players died suffering";
        bool win;
        if (isVrPlayer)
        {
            win = outsides == 0;
        }
        else
        {
            win = gameObject.GetComponent<State>().stateProp == PlayerState.Outside;
        }
        
        generalUI.SetActive(true);
        winning.SetActive(win);
        losing.SetActive(!win);
    }
    
    
    
}
