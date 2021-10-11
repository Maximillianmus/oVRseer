using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Mirror;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spectator : NetworkBehaviour
{
    private List<GameObject> players = new List<GameObject>();
    private int indexPlayer = 0;
    private GameObject spectating = null;
    [SyncVar]
    public bool IsDead = false;
    public StarterAssetsInputs _inputs;
    public PlayerInput deadInput;
    public List<GameObject> toDisableOnDead;
    public List<GameObject> toEnableOnDead;
    public List<GameObject> toEnableMobileOnDead;
    public GameObject EndScreen;
    public GameObject deadUI;
    public NetworkNickname nicknameScript;
    
    private void RefreshPlayers()
    {
        players.Clear();
        var possiblePlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
        foreach (var player in possiblePlayers)
        {
            if (!player.GetComponent<Spectator>().IsDead && player != gameObject)
            {
                players.Add(player);
            } 
        }
    }

    private void Start()
    {
       RefreshPlayers();
       IsDead = false;
    }

    private void Update()
    {
        if (isServer)
        {
            return;
        }
        if (!IsDead && _inputs.dead && hasAuthority)
        {
            OnDead();
            return;
        }

        // local player 
        NetworkIdentity netID = GetComponent<NetworkIdentity>();

        if (IsDead && netID.hasAuthority)
        {
            RefreshAndCheck();
            if (players.Count == 0)
            {
                CmdAllPlayersDead();
            }
        }
        
    }

    [Command]
    private void CmdAllPlayersDead()
    {
        RpcTotalEndScreen();
    }

    [ClientRpc]
    private void RpcTotalEndScreen()
    {
        EndScreen.SetActive(true);
    }

    public void OnDead()
    {
        deadInput.enabled = true;
        IsDead = true;
        RefreshPlayers();
        foreach (var toDisable in toDisableOnDead)
        {
            toDisable.SetActive(false);
        }

        foreach (var toEnable in toEnableOnDead)
        {
            toEnable.SetActive(true);
        }
        #if UNITY_ANDROID
        foreach (var enableMobile in toEnableMobileOnDead)
        {
            enableMobile.SetActive(true);
        }
        #endif
        OnPlayerDead();
        if (players.Count == 0)
        {
            return;
        }
        
        SwitchSpectatePlayer(-1);
    }


    private void SwitchSpectatePlayer(int oldIndex, int step = 1)
    {
        if (step == 0)
        {
            return;
        }
        if (players.Count == 0)
        {
            return;
        }

        GameObject oldPlayer;
        if (oldIndex >= 0)
        {
            oldPlayer = players[oldIndex];
        }
        else
        {
            oldPlayer = gameObject;
        }
        
        oldPlayer.GetComponentInChildren<Camera>(true).gameObject.SetActive(false);
        oldPlayer.GetComponentInChildren<CinemachineVirtualCamera>(true).gameObject.SetActive(false);

        var newIndex = (oldIndex + step) % players.Count;
        var newPlayer = players[newIndex];
        spectating = newPlayer;
        newPlayer.GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
        newPlayer.GetComponentInChildren<CinemachineVirtualCamera>(true).gameObject.SetActive(true);
        var nickSpectate = newPlayer.GetComponentInChildren<NetworkNickname>(true).nickname;
        deadUI.GetComponent<changeNickName>().ChangeNickName(nickSpectate);
        indexPlayer = newIndex;
    }

    public void OnNextPlayer()
    {
        SwitchSpectatePlayer(indexPlayer);
    }

    public void OnPreviousPlayer()
    {
        SwitchSpectatePlayer(indexPlayer, -1);
    }

    public void OnPlayerDead()
    {
        CmdNotifyPlayerDead();
    }

    [Command]
    void CmdNotifyPlayerDead()
    {
        IsDead = true;
    }

    private void RefreshAndCheck()
    {
        RefreshPlayers();
        if (players.Count == 0 || !IsDead)
        {
            return;
        }
        var player = players.FindIndex(x => x == spectating);
        if (player == -1)
        {
            SwitchSpectatePlayer(-1); 
        }
        else
        {
            indexPlayer = player;
        }
    }
}
