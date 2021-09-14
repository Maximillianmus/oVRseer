using System;
using System.Collections.Generic;
using kcp2k;
using Mirror;
using UnityEngine;

namespace Network
{
    public class CompteurJoueur
    {
        public int total;
        public int ready;
        public int overseerReady;
        public int tinyReady;

    }

    
    
    public class OVRseerNetworkManager : Mirror.NetworkManager
    {
        public GameObject overSeerPrefab;
        public GameObject gamePlayerPrefer;

        [Scene] public string gameScene;

        public List<OVRseerRoomPlayer> roomPlayers { get; } = new List<OVRseerRoomPlayer>();

        private bool canLaunch(CompteurJoueur compteurJoueur)
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

            compteurJoueur.ready = nbPlayer;
            compteurJoueur.total = roomPlayers.Count;
            compteurJoueur.overseerReady = nbOverseer;
            compteurJoueur.tinyReady = nbTiny;
            
            return nbPlayer == roomPlayers.Count && nbTiny >= 1 && nbOverseer >= 1;
        }

        public void ChangeStatusClient()
        {
            CompteurJoueur compteur = new CompteurJoueur();
            if (canLaunch(compteur))
            {
                RpcLeaderStartStatus(true, compteur);
            }
            else
            {
                RpcLeaderStartStatus(false, compteur);
            }

        }

        public void RpcLeaderStartStatus(bool canStart, CompteurJoueur compteurJoueur)
        {
            foreach (OVRseerRoomPlayer roomPlayer in roomPlayers)
            {
                    roomPlayer.NotifyCanStart(canStart, compteurJoueur);
            }
            
        }




    }

}