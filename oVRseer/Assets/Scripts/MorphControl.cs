using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using StarterAssets;
using UnityEngine;

public class MorphControl : MonoBehaviour
{
    private StarterAssetsInputs _inputs;
    public GameObject baseMesh;
    public GameObject morphMesh;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _inputs = GetComponent<StarterAssetsInputs>();

    }

    // Update is called once per frame
    void Update()
    {
        baseMesh.SetActive(!_inputs.morph);
        morphMesh.SetActive(_inputs.morph);
        if (_inputs.choose) 
            RayCast();
    }


    public void RayCast()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        if (!hit)
        {
            return;
        }

        var morphMeshFilter = morphMesh.GetComponent<MeshFilter>();
        Destroy(morphMesh.GetComponent<MeshCollider>());
        morphMeshFilter.mesh = hitInfo.transform.gameObject.GetComponent<MeshFilter>().mesh;
        var morphCollider = morphMesh.AddComponent<MeshCollider>();
        morphCollider.sharedMesh = morphMeshFilter.mesh;
    }
    
}
