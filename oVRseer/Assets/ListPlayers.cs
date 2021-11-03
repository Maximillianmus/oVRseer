using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public struct PlayerInfo
{
    public GameObject go;
    public string nick;
}

public struct PlayerCount : NetworkMessage
{
    public int squashed;
    public int outsides;
}

public class ListPlayers : NetworkBehaviour
{
    private int totalPlayer, tinyInside, tinyOutside, tinySquashed;
    private List<PlayerInfo> playersInfo = new List<PlayerInfo>();

    private void Refresh()
    {
        playersInfo.Clear();
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            var stateScr = player.GetComponent<State>();
            switch (stateScr.stateProp)
            {
                case PlayerState.Inside:
                    tinyInside += 1;
                    break;
                case PlayerState.Outside:
                    tinyOutside += 1;
                    break;
                case PlayerState.Squashed:
                    tinySquashed += 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            totalPlayer += 1;
            
            PlayerInfo newPlayer;
            newPlayer.go = player;
            newPlayer.nick = player.GetComponent<NetworkNickname>().nickname;
            playersInfo.Add(newPlayer);
        }


        if (tinyInside == 0)
        {
            PlayerCount playerCount;
            playerCount.outsides = tinyOutside;
            playerCount.squashed = tinySquashed;
            CmdPlayerCount(playerCount);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdPlayerCount(PlayerCount playerCount)
    {
        NetworkServer.SendToAll(playerCount);
    }


    public void OnPlayerChangeState()
    {
        Refresh();
    }

}
