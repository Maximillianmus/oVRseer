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
    public int numOfKeysToSpawn = 1; // Hard coded for now

    private void Start()
    {
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