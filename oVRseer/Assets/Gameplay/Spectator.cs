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
    private GameObject spectating = null;
    [SyncVar]
    public bool IsDead = false;
    public StarterAssetsInputs _inputs;
    public PlayerInput deadInput;
    public GameObject playerArmature;
    public List<GameObject> toDisableOnDead;
    public List<GameObject> toEnableOnDead;
    public List<GameObject> toEnableMobileOnDead;
    public GameObject deadUI;
    
    private void RefreshPlayers()
    {
        players.Clear();
        var possiblePlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
        foreach (var player in possiblePlayers)
        {
            if (player.GetComponent<State>().isPlaying() && player != gameObject)
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
        }
        
    }


    public void OnDead()
    {
        if (!hasAuthority)
        {
            return;
        }
        deadInput.enabled = true;
        IsDead = true;
        RefreshPlayers();
        foreach (var toDisable in toDisableOnDead)
        {
            toDisable.SetActive(false);
        }

        CmdDisablePA();
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

        spectating = gameObject;
        SwitchSpectatePlayer();
    }

    [Command]
    private void CmdDisablePA()
    {
        RpcDisablePA();
    }

    [ClientRpc]
    private void RpcDisablePA()
    {
        this.playerArmature.SetActive(false);
    }


    private void DisableCamera(GameObject gameObject)
    {
        gameObject.GetComponentInChildren<Camera>(true).gameObject.SetActive(false);
        gameObject.GetComponentInChildren<CinemachineVirtualCamera>(true).gameObject.SetActive(false);
    }

    private void EnableCamera(GameObject gameObject)
    {
        spectating = gameObject;
        gameObject.GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
        gameObject.GetComponentInChildren<CinemachineVirtualCamera>(true).gameObject.SetActive(true);
        var nickSpectate = gameObject.GetComponentInChildren<NetworkNickname>(true).nickname;
        deadUI.GetComponent<changeNickName>().ChangeNickName(nickSpectate);
    }
    
    private void SwitchSpectatePlayer(int step = 1)
    {
        if (!hasAuthority || step == 0 || players.Count == 0)
        {
            return;
        }
        DisableCamera(spectating);
        var index = players.FindIndex(x => x == spectating);
        var newIndex = index == -1 ? 0 : mod(index + step, players.Count);
        EnableCamera(players[newIndex]);
        spectating = players[newIndex];

    }

    private int mod(int a, int m)
    {
        return ((a %= m) < 0) ? a + m : a;
    }

    public void OnPhoneNextPlayer()
    {
       SwitchSpectatePlayer(); 
    }
    
    public void OnPhonePreviousPlayer()
    {
       SwitchSpectatePlayer(-1); 
    }
    
    
    public void OnNextPlayer(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase == InputActionPhase.Performed)
            SwitchSpectatePlayer();
    }

    public void OnPreviousPlayer(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase == InputActionPhase.Performed)
            SwitchSpectatePlayer(-1);
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
            SwitchSpectatePlayer(); 
        }
    }
}
