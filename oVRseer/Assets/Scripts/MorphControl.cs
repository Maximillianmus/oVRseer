using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Mirror;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class MorphControl : NetworkBehaviour
{
    public StarterAssetsInputs _inputs;
    public GameObject baseMesh;
    public GameObject morphMesh;
    public ActionCD morphCD;
    private bool lastMorphState = false;
    
    

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (_inputs.morph != lastMorphState)
        {
            lastMorphState = _inputs.morph;
            morphCD.OnActionPress(Time.time, _inputs.morphCooldown);
            CmdMorphAbility(_inputs.morph);
        }
        
        if (_inputs.choose) 
            RayCast();
    }

    [Command]
    void CmdMorphAbility(bool toMorph)
    {
        RpcMorphAbility(toMorph);
    }

    [ClientRpc]
    void RpcMorphAbility(bool toMorph)
    {
        baseMesh.SetActive(!toMorph);
        morphMesh.SetActive(toMorph);
        
    }


    public void RayCast()
    {
        RaycastHit hitInfo = new RaycastHit();
        LayerMask layerMask = LayerMask.GetMask("Map", "Ground");
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity,
            layerMask);
        if (!hit)
        {
            return;
        }

        NetworkIdentity networkIdentity;
        bool hasNetworkIdentity = hitInfo.transform.gameObject.TryGetComponent<NetworkIdentity>(out networkIdentity);
        if (!hasNetworkIdentity)
        {
            Debug.Log("this object can not be morph through network");
            return;
        }

        CmdChangeMorphTo(networkIdentity.netId);
    }

    [Command]
    void CmdChangeMorphTo(uint netId)
    {
        RpcChangeMorphTo(netId);
    }

    [ClientRpc]
     void RpcChangeMorphTo(uint netId)
     {
         var newMeshObj = NetworkClient.spawned[netId];
         var morphMeshFilter = morphMesh.GetComponent<MeshFilter>();
         Destroy(morphMesh.GetComponent<MeshCollider>());
         morphMeshFilter.mesh = newMeshObj.gameObject.GetComponent<MeshFilter>().mesh;
         var morphCollider = morphMesh.AddComponent<MeshCollider>();
         morphCollider.sharedMesh = morphMeshFilter.mesh;
         morphCollider.convex = true;
     }

    
}
