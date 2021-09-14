using System;
using Mirror;
using Network;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerType
{
    Tiny,
    Overseer
}

public class OVRseerRoomPlayer : NetworkBehaviour
{
    private const int MAX_OVERSEER = 1;
    
    public GameObject lobbyUI;
    [SerializeField] GameObject startButton;

    [SerializeField] public Text nbJoueursReadyText;
    [SerializeField] public Text nbJoueursTotalText;
    
    [SerializeField] public Button overseerButton;
    [SerializeField] public Button tinyButton;
    [SerializeField] public GameObject ReadyText;
    
    [SyncVar(hook = nameof(HandleCompteurChanged))]
    private int nbTotalReady;
    [SyncVar(hook = nameof(HandleCompteurChanged))]
    private int nbTotalPlayer;

    [SyncVar(hook = nameof(HandleCompteurChanged))]
    private int nbOverseer;

    public void HandleCompteurChanged(int oldValue, int newValue) => updateDisplay();
    public void isReadyChanged(bool oldValue, bool newValue) => updateDisplay();
    
    [SyncVar(hook = nameof(isReadyChanged))]
    public bool isReady;

    private void updateDisplay()
    {
        nbJoueursReadyText.text = nbTotalReady.ToString();
        nbJoueursTotalText.text = nbTotalPlayer.ToString();

        ReadyText.SetActive(isReady);
        overseerButton.interactable = (isReady && type == PlayerType.Overseer) || !isReady;
        tinyButton.interactable = (isReady && type == PlayerType.Tiny) || !isReady;



        if (nbOverseer >= MAX_OVERSEER && type != PlayerType.Overseer)
        {
            overseerButton.interactable = false;
        }
    }
    
    [SyncVar]
    public PlayerType type = PlayerType.Tiny;

    
    private bool isLeader;
    public bool IsLeader
    {
        get
        {
            return isLeader;
        }
    }

    private OVRseerNetworkManager room;
    private OVRseerNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as OVRseerNetworkManager;
        }
    }

    public override void OnStartAuthority()
    {
        lobbyUI.SetActive(true);
        base.OnStartAuthority();
    }

    public override void OnStartClient()
    {
        if (Room.roomPlayers.Count == 0)
        {
            isLeader = true;
        }
        Room.roomPlayers.Add(this);
        if (hasAuthority) 
            CmdNotifyExist();
    }

    public void ReadyAsOverseer()
    {
        CmdReadyAsOverseer();
    }
    
    public void ReadyAsTiny()
    {
        CmdReadyAsTiny();
    }

    [Command]
    private void CmdReadyAsOverseer()
    {
        if (isReady)
        {
            isReady = false;
        }
        else
        {
            isReady = true;
            type = PlayerType.Overseer;
        }

        Room.ChangeStatusClient();

    }

    [Command]
    private void CmdReadyAsTiny()
    {
        if (isReady)
        {
            isReady = false;
        }
        else
        {
            isReady = true;
            type = PlayerType.Tiny;
        }
        Room.ChangeStatusClient();
    }

    [Command]
    public void CmdNotifyExist()
    {
        Room.ChangeStatusClient();
    }

    public void NotifyCanStart(bool canStart, CompteurJoueur compteurJoueur)
    {
        nbTotalPlayer = compteurJoueur.total;
        nbTotalReady = compteurJoueur.ready;
        nbOverseer = compteurJoueur.overseerReady;
        
        
        
        if (isLeader)
        {
            startButton.SetActive(canStart);
        }
    }
    
}
