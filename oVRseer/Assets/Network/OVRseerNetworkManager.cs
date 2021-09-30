using System;
using System.Collections.Generic;
using kcp2k;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Network
{
    public class CountPlayer
    {
        public int total;
        public int ready;
        public int overseerReady;
        public int tinyReady;

    }

    public struct DisplayAlert : NetworkMessage
    {
        public bool toDisplay;
    }

    public struct RoomPlayersToDestroy
    {
        public GameObject roomPlayer;
        public NetworkConnection conn;
    }
    
    
    public class OVRseerNetworkManager : Mirror.NetworkManager
    {
        [SerializeField] GameObject gamePlayerPrefab;


        [SerializeField] private GameObject keySpawnSystem = null;
        [SerializeField] public GameObject keyPrefab;

        [Scene] public string gameScene;
        [SerializeField] private GameObject playerSpawnSystem = null;
        [SerializeField] public Text serverAdress;
        [SerializeField] public GameObject alertStarted;

        public static event Action<NetworkConnection> onServerReadied;

        public List<OVRseerRoomPlayer> roomPlayers { get; } = new List<OVRseerRoomPlayer>();
        private List<RoomPlayersToDestroy> _roomPlayersToDestroys = new List<RoomPlayersToDestroy>();

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

            return nbPlayer == roomPlayers.Count && nbTiny >= 1 && nbOverseer >= 0; ///!!!
        }
        
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if (conn.identity != null && IsSceneActive(onlineScene))
            {
                var player = conn.identity.GetComponent<OVRseerRoomPlayer>();

                roomPlayers.Remove(player);

                var count = new CountPlayer();
                NotifyStartStatus(canLaunch(count), count);
            }

            base.OnServerDisconnect(conn);
        }
        
        public override void OnServerConnect(NetworkConnection conn)
        {
            if (numPlayers >= maxConnections)
            {
                conn.Disconnect();
                return;
            }

            if (!IsSceneActive(onlineScene))
            {
                conn.Disconnect();
                return;
            }
        }

        public override void OnStopServer()
        {
            roomPlayers.Clear();
        }

        public void StartClientAdress()
        {
            alertStarted.SetActive(false);
            if (serverAdress.text.Length != 0)
            {
                networkAddress = serverAdress.text;
            }

            StartClient();
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

        
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            if (IsSceneActive(onlineScene))
            {
                bool isLeader = roomPlayers.Count == 0;

                GameObject roomPlayerInstance = Instantiate(playerPrefab);

                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            }
        }
        
        public override void OnServerSceneChanged(string sceneName)
        {
            if (IsSceneActive(gameScene))
            {
                GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
                NetworkServer.Spawn(playerSpawnSystemInstance);

                GameObject keySpawnSystemInstance = Instantiate(keySpawnSystem);
                NetworkServer.Spawn(keySpawnSystemInstance);

            }
        }

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);
            onServerReadied?.Invoke(conn);
            if (_roomPlayersToDestroys.Count == roomPlayers.Count)
            {
                DestroyAllRoomPlayers();
            }
        }

        private void DestroyAllRoomPlayers()
        {
            for (int i = _roomPlayersToDestroys.Count - 1; i >= 0; --i)
            {
                NetworkServer.Destroy(_roomPlayersToDestroys[i].roomPlayer);
            }
            _roomPlayersToDestroys.Clear();
        }

        public void ReplacePlayer(NetworkConnection conn, GameObject newInstance)
        {
            // Cache a reference to the current player object
            GameObject oldPlayer = conn.identity.gameObject;

            // Instantiate the new player object and broadcast to clients
            NetworkServer.ReplacePlayerForConnection(conn, newInstance, true);

            RoomPlayersToDestroy roomPlayersToDestroy = new RoomPlayersToDestroy
            {
                conn = conn,
                roomPlayer = oldPlayer
            };
            _roomPlayersToDestroys.Add(roomPlayersToDestroy);

        }
    }
    

}