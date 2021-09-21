using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

public class disableVR : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
