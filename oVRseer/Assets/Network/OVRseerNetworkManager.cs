using System;
using kcp2k;
using Mirror;
using UnityEngine;

namespace NetworkThings
{

    public enum PlayerType
    {
        Tiny,
        Overseer
    };

    struct CreatePlayerMessage : Mirror.NetworkMessage
    {
        public PlayerType type;
    }
    
    
    public class OVRseerNetworkManager : Mirror.NetworkManager
    {
        private int NbOverseer;
        private int NbTiny;

        public GameObject overSeerPrefab;

        private PlayerType PlayerType = PlayerType.Tiny;
        
        
        
        public override void OnStartServer()
        {
            base.OnStartServer();
            NbOverseer = 0;
            NbTiny = 0;
            NetworkServer.RegisterHandler<CreatePlayerMessage>(OnPlayerCreation);
        }

        public void chooseOverseer()
        {
            PlayerType = PlayerType.Overseer;
            StartClient();
        }

        public void chooseTiny()
        {
            PlayerType = PlayerType.Tiny;
            StartClient();
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            CreatePlayerMessage playerMessage = new CreatePlayerMessage
            {
                type = PlayerType,
            };
            conn.Send(playerMessage); 
        }

        void OnPlayerCreation(NetworkConnection conn, CreatePlayerMessage playerMessage)
        {
            Log.Error("essai");
            GameObject ToSpawn = playerMessage.type == PlayerType.Overseer ? overSeerPrefab : playerPrefab;
            GameObject gameObject = Instantiate(ToSpawn);
            switch (playerMessage.type)
            {
                case PlayerType.Tiny:
                    NbTiny += 1;
                    break;
                case PlayerType.Overseer:
                    NbOverseer += 1;
                    break;
                default:
                    Log.Warning("Try to spawn of player with no defined type : Tiny spawned by default");
                    break;
            }

            NetworkServer.AddPlayerForConnection(conn, gameObject);
        }

    }
}