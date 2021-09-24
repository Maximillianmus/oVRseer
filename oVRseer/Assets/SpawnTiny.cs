using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTiny : NetworkBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject CameraPrefab;

    private NetworkIdentity networkId;
    private NetworkConnectionToClient networkConnectionToClient;

    // Start is called before the first frame update
    public override void OnStartLocalPlayer()
    {
        networkId = transform.GetComponent<NetworkIdentity>();
        networkConnectionToClient = networkId.connectionToClient;

        GameObject Camera = Instantiate(CameraPrefab, transform.position, transform.rotation);
        CmdSpawnModel();

    }



    //spawns the player model with authorithy for the local player
    [Command]
    void CmdSpawnModel()
    {
        //instantiaties the object where the networkplayer is
        GameObject playerModel = Instantiate(PlayerPrefab, transform.position, transform.rotation);

        NetworkServer.Spawn(playerModel, connectionToClient);
        print("hello");
    }



}
