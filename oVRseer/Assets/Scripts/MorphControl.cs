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
    public Image morphButton;
    
    

    // Update is called once per frame
    void Update()
    {
        var buttonColor = morphButton.color;
        if (Time.time - _inputs.lastTimeMorph < _inputs.morphCooldown)
        {
            morphButton.color = Color.gray;
            buttonColor.a = 255;
        }
        else
        {
            morphButton.color = Color.white;
            buttonColor.a = 0;
        } 
        CmdMorphAbility(_inputs.morph);
        
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
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, layerMask);
        if (!hit)
        {
            return;
        }

        var morphMeshFilter = morphMesh.GetComponent<MeshFilter>();
        Destroy(morphMesh.GetComponent<MeshCollider>());
        morphMeshFilter.mesh = hitInfo.transform.gameObject.GetComponent<MeshFilter>().mesh;
        var morphCollider = morphMesh.AddComponent<MeshCollider>();
        morphCollider.sharedMesh = morphMeshFilter.mesh;
        morphCollider.convex = true;
    }
    
}
