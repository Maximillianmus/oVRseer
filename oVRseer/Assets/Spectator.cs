using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spectator : MonoBehaviour
{
    private List<GameObject> players = new List<GameObject>();
    public GameObject playerArmature;
    private int indexPlayer = 0;
    public bool IsDead = false;
    public StarterAssetsInputs _inputs;
    
    private void RefreshPlayers()
    {
        players.Clear();
        var possiblePlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
        foreach (var player in possiblePlayers)
        {
            if (!player.GetComponentInChildren<Spectator>().IsDead && player != transform.parent.gameObject)
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
        if (!IsDead && _inputs.dead)
        {
            OnDead();
        }
        
    }

    public void OnDead()
    {
        GetComponent<PlayerInput>().enabled = true;
        playerArmature.SetActive(false);
        IsDead = true;
        RefreshPlayers();
        if (players.Count == 0)
        {
            return;
        }
        
        SwitchSpectatePlayer(-1);
    }


    private void SwitchSpectatePlayer(int oldIndex)
    {
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
            oldPlayer = transform.parent.gameObject;
        }
        
        oldPlayer.GetComponentInChildren<Camera>().gameObject.SetActive(false);
        oldPlayer.GetComponentInChildren<CinemachineVirtualCamera>().gameObject.SetActive(false);

        var newIndex = (oldIndex + 1) % players.Count;
        var newPlayer = players[newIndex];
        newPlayer.GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
        newPlayer.GetComponentInChildren<CinemachineVirtualCamera>(true).gameObject.SetActive(true);
        indexPlayer = newIndex;
    }

    public void OnNextPlayer()
    {
        SwitchSpectatePlayer(indexPlayer);
    }

}
