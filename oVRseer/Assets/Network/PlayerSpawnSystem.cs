using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Network
{
    public class PlayerSpawnSystem : NetworkBehaviour
    {
        [SerializeField] GameObject tinyPrefab = null;
        [SerializeField] GameObject overSeerPrefab = null;

        private static List<Transform> tinyPositions = new List<Transform>();
        private static List<Transform> overseerPositions = new List<Transform>();

        private int tinyIndex = 0;
        private int overseerIndex = 0;

        private OVRseerNetworkManager room;
        private OVRseerNetworkManager Room
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as OVRseerNetworkManager;
            }
        }
        
        public static void AddTinySpawnPoint(Transform tinyPosition)
        {
            tinyPositions.Add(tinyPosition);
        }
        
        public static void AddOverseerSpawnPoint(Transform overseerPosition)
        {
            overseerPositions.Add(overseerPosition);
        }
        
        
        public static void RemoveTinySpawnPoint(Transform tinyPosition)
        {
            tinyPositions.Remove(tinyPosition);
        }
        
        public static void RemoveOverseerSpawnPoint(Transform overseerPosition)
        {
            overseerPositions.Remove(overseerPosition);
        }

        public override void OnStartServer()
        {
            OVRseerNetworkManager.onServerReadied += SpawnPlayer;
        }

        [ServerCallback]
        private void OnDestroy()
        {
            OVRseerNetworkManager.onServerReadied -= SpawnPlayer;
        }

        public void SpawnPlayer(NetworkConnection conn)
        {
            var roomPlayer = conn.identity.gameObject.GetComponent<OVRseerRoomPlayer>();
            var roomObject = conn.identity.gameObject;
            GameObject ToSpawn = null;
            switch (roomPlayer.type)
            {
                case PlayerType.Tiny:
                    var totalTinyPosition = tinyPositions.Count;
                    if (tinyIndex >= totalTinyPosition)
                    {
                        Debug.LogWarning("There is not enough position for tiny players as players : the position will circle");
                    }
                    ToSpawn = Instantiate(tinyPrefab, tinyPositions[tinyIndex % totalTinyPosition].position,
                        tinyPositions[tinyIndex % totalTinyPosition].rotation);
                    tinyIndex++;
                    break;
                case PlayerType.Overseer:
                    var totalOverseerPosition = overseerPositions.Count;
                    if (overseerIndex >= totalOverseerPosition)
                    {
                        Debug.LogWarning("There is not enough position for overseer players as players : the position will circle");
                    }
                    ToSpawn = Instantiate(overSeerPrefab, overseerPositions[overseerIndex % totalOverseerPosition].position,
                        tinyPositions[overseerIndex % totalOverseerPosition].rotation);
                    overseerIndex++;
                    break;
                default:
                    break;
            }

            Room.ReplacePlayer(conn, ToSpawn);

        }

    }
}