using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTiny : NetworkBehaviour
{
    public GameObject PlayerPrefab;

    private NetworkIdentity networkId;
    private NetworkConnectionToClient networkConnectionToClient;

    // Start is called before the first frame update
    public override void OnStartLocalPlayer()
    {
        networkId = transform.GetComponent<NetworkIdentity>();
        networkConnectionToClient = networkId.connectionToClient;

        CmdSpawnModel();


    }



    //spawns the player model with authorithy for the local player
    [Command]
    void CmdSpawnModel()
    {
        //instantiaties the object where the networkplayer is
        GameObject playerModel = Instantiate(PlayerPrefab, transform.position, transform.rotation);
        playerModel.GetComponent<NetworkNickname>().nickname = GetComponent<OVRseerNetworkGamePlayer>().nickname;

        NetworkServer.Spawn(playerModel, connectionToClient);
        print("hello");
    }



}
