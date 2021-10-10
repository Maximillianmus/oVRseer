using System;
using Mirror;
using Network;
using Telepathy;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerType
{
    Tiny,
    Overseer
}

public class OVRseerRoomPlayer : NetworkBehaviour
{
    // *** Constante ***
    private const int MAX_OVERSEER = 1;
    
    [Header("UI")]
    public GameObject lobbyUI;
    [SerializeField] GameObject startButton;
    [SerializeField] public Button overseerButton;
    [SerializeField] public Button tinyButton;
    [SerializeField] public Text nbJoueursReadyText;
    [SerializeField] public Text nbJoueursTotalText;
    [SerializeField] public GameObject ReadyText;
    // public Text nicknameInput;
    [SyncVar] public string nickname;
    public Text nickInput;
    
    // *** Sync Var for number of players ***
    [SyncVar(hook = nameof(HandleCompteurChanged))]
    private int nbTotalReady;
    [SyncVar(hook = nameof(HandleCompteurChanged))]
    private int nbTotalPlayer;
    [SyncVar(hook = nameof(HandleCompteurChanged))]
    private int nbOverseer;
    
    
    [SyncVar(hook = nameof(isReadyChanged))]
    public bool isReady;
    
    [SyncVar]
    public PlayerType type = PlayerType.Tiny;

    // *** Handlers *** 
    public void HandleCompteurChanged(int oldValue, int newValue) => updateDisplay();
    public void isReadyChanged(bool oldValue, bool newValue) => updateDisplay();
    
    
    
    
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
    

    /// <summary>
    /// update the UI function of the state given by numbers and ready
    /// </summary>
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
    

    /// <summary>
    /// Active the UI if the program has authority (local player)
    /// </summary>
    public override void OnStartAuthority()
    {
        lobbyUI.SetActive(true);
        base.OnStartAuthority();
    }

    /// <summary>
    /// Add the player to the rooms and notify
    /// </summary>
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


    private void Start()
    {
        if (Room.dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        } 
    }

    public override void OnStopClient()
    {
        Room.roomPlayers.Remove(this);
        if (hasAuthority)
        {
            // CmdNotifyExist();
        }
    }

    // *** Ready functions & commands ***

    [Command]
    public void CmdReadyAsOverseer()
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

    public void OnEndEditNick(String newNickname)
    {
        CmdEditNick(nickInput.text);
    }

    [Command]
    void CmdEditNick(String newnick)
    {
        nickname = newnick;
    }

    [Command]
    public void CmdReadyAsTiny()
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

    
    public void Disconnect()
    {
        if (isServer && isClient)
        {
            Room.StopHost();
            return;
        }

        if (isServer)
        {
            Room.StopServer();
            return;
        }

        if (isClient)
        {
            Room.StopClient();
            return;
        }
    }

    /// <summary>
    /// Update numbers and active the possibility of starting the game if leader
    /// Called at each update of ready
    /// </summary>
    /// <param name="canStart"></param>
    /// <param name="countPlayer"></param>
    public void NotifyCanStart(bool canStart, CountPlayer countPlayer)
    {
        nbTotalPlayer = countPlayer.total;
        nbTotalReady = countPlayer.ready;
        nbOverseer = countPlayer.overseerReady;
        
        if (isLeader)
        {
            startButton.SetActive(canStart);
        }
    }

    [Command]
    public void CmdStartGame()
    {
        Room.StartGame();
    }
}
