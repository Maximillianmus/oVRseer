using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Network;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class KeySpawnSystem : NetworkBehaviour
{

    [SerializeField] GameObject keyPrefab = null;
    private static List<Transform> keyPositions = new List<Transform>();

    public List<GameObject> keysInScene = new List<GameObject>();
    [SyncVar] public int numOfKeysToSpawn; // Hard coded for now
    [SyncVar] public int numOfKeysToCollect;
    [SyncVar] public int numOfPlayers = 0;

    private OVRseerNetworkManager room;
    private OVRseerNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as OVRseerNetworkManager;
        }
    }

    public override void OnStartServer()
    {
        OVRseerNetworkManager.onServerAllReadied += spawnKey;
    }

    [ServerCallback]
    private void OnDestroy()
    {
        OVRseerNetworkManager.onServerAllReadied -= spawnKey;
    }


    [Server]
    public void spawnKey(int numberOfPlayersServer)
    {

        if (SceneManager.GetActiveScene().name != Room.gameScene &&
            SceneManager.GetActiveScene().path != Room.gameScene) return;
        numOfPlayers = numberOfPlayersServer;
        if (numOfPlayers == 0)
        {
            Debug.LogError("The number of players can not be 0");
        }
        numOfKeysToCollect = numOfPlayers + 1;

        // We have a maximum number of spawnpositions so if we have alot of players the number of keys needed to collect will reach a limit
        if (numOfKeysToCollect >= keyPositions.Count / 2)
        {
            numOfKeysToCollect = (keyPositions.Count / 2);
        }

        numOfKeysToSpawn = 2 * numOfKeysToCollect;
        SpawnKeys();
    }


    public static void AddKeySpawnPoint(Transform keyPosition)
    {
        keyPositions.Add(keyPosition);
    }

    public static void RemoveKeySpawnPoint(Transform keyPosition)
    {
        keyPositions.Remove(keyPosition);
    }

    public void SpawnKeys()
    {

        for (int i = 0; i < numOfKeysToSpawn; i++)
        {

            if (keyPositions.Count > 0)
            {

                int spawnPositon = Random.Range(0, keyPositions.Count);
                GameObject keyToSpawn = Instantiate(keyPrefab, keyPositions[spawnPositon].position, keyPositions[spawnPositon].rotation);
                NetworkServer.Spawn(keyToSpawn);
                keysInScene.Add(keyToSpawn);
                keyPositions.RemoveAt(spawnPositon);
            }
        }

    }

}