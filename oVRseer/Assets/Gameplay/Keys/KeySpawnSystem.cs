using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Network;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KeySpawnSystem : NetworkBehaviour
{

    [SerializeField] GameObject keyPrefab = null;
    private static List<Transform> keyPositions = new List<Transform>();

    public List<GameObject> keysInScene = new List<GameObject>();
    public int numOfKeysToSpawn; // Hard coded for now
    public int numOfKeysToCollect;
    public int numOfPlayers;

    public override void OnStartServer()
    {

    }

    public void Start()
    {
        FindNumPlayers();
        numOfKeysToCollect = numOfPlayers + 1;
        numOfKeysToSpawn = 2 * numOfKeysToCollect;
        SpawnKeys();
    }

    private void FindNumPlayers()
    {
        foreach (GameObject k in GameObject.FindGameObjectsWithTag("Player"))
        {
            numOfPlayers++;
        }
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