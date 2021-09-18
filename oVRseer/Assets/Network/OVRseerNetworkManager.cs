﻿using System;
using System.Collections.Generic;
using kcp2k;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class CountPlayer
    {
        public int total;
        public int ready;
        public int overseerReady;
        public int tinyReady;

    }
    
    
    
    public class OVRseerNetworkManager : Mirror.NetworkManager
    {
        [SerializeField] GameObject gamePlayerPrefab;
        
        [Scene] public string gameScene;
        [SerializeField] private GameObject playerSpawnSystem = null;

        public static event Action<NetworkConnection> onServerReadied;
        


        public List<OVRseerRoomPlayer> roomPlayers { get; } = new List<OVRseerRoomPlayer>();

        /// <summary>
        /// Check if game can be launched and update count
        /// </summary>
        /// <param name="countPlayer"></param>
        /// <returns></returns>
        private bool canLaunch(CountPlayer countPlayer)
        {
            int nbPlayer = 0;
            int nbOverseer = 0;
            int nbTiny = 0;
            foreach (OVRseerRoomPlayer roomPlayer in roomPlayers)
            {
                if (roomPlayer.isReady)
                {
                    nbPlayer += 1;
                    switch (roomPlayer.type)
                    {
                        case PlayerType.Tiny:
                            nbTiny += 1;
                            break;
                        case PlayerType.Overseer:
                            nbOverseer += 1;
                            break;
                        default:
                            break;
                    }
                }
            }

            if (countPlayer != null)
            {
                countPlayer.ready = nbPlayer;
                countPlayer.total = roomPlayers.Count;
                countPlayer.overseerReady = nbOverseer;
                countPlayer.tinyReady = nbTiny;
            }

            return nbPlayer == roomPlayers.Count && nbTiny >= 1 && nbOverseer >= 1;
        }
        
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if (conn.identity != null)
            {
                var player = conn.identity.GetComponent<OVRseerRoomPlayer>();

                roomPlayers.Remove(player);

                var count = new CountPlayer();
                NotifyStartStatus(canLaunch(count), count);
            }

            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            roomPlayers.Clear();
        } 
        

        /// <summary>
        /// Called each time a client change its ready status
        /// </summary>
        public void ChangeStatusClient()
        {
            CountPlayer compteur = new CountPlayer();
            if (canLaunch(compteur))
            {
                NotifyStartStatus(true, compteur);
            }
            else
            {
                NotifyStartStatus(false, compteur);
            }

        }

        /// <summary>
        /// Notify each player of the room for the status of the game and the count 
        /// </summary>
        /// <param name="canStart"></param>
        /// <param name="countPlayer"></param>
        public void NotifyStartStatus(bool canStart, CountPlayer countPlayer)
        {
            foreach (OVRseerRoomPlayer roomPlayer in roomPlayers)
            {
                    roomPlayer.NotifyCanStart(canStart, countPlayer);
            }
            
        }

        public void StartGame()
        {
            if (IsSceneActive(onlineScene))
            {
                if (!canLaunch(null))
                {
                    return;
                }

                ServerChangeScene(gameScene);
            }
        }

        public override void ServerChangeScene(string newSceneName)
        {
            // From menu to game
            if (IsSceneActive(onlineScene) && newSceneName.Contains(gameScene))
            {
                for (int i = roomPlayers.Count - 1; i >= 0; i--)
                {
                    var conn = roomPlayers[i].connectionToClient;
                    var roomObject = conn.identity.gameObject;
                    var type = roomPlayers[i].type;
                    var gameplayerInstance = Instantiate(gamePlayerPrefab);
                    var gameScript = gameplayerInstance.GetComponent<OVRseerNetworkGamePlayer>();
                    gameScript.type = type;

                    NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject, true);
                    NetworkServer.Destroy(roomObject);

                }
            }
            base.ServerChangeScene(newSceneName);
            
        }
        
        
        public override void OnServerSceneChanged(string sceneName)
        {
            if (IsSceneActive(gameScene))
            {
                GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
                NetworkServer.Spawn(playerSpawnSystemInstance);
            }
        }

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);
            onServerReadied?.Invoke(conn);
        }
    }
    

}