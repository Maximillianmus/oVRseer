using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.XR.Management;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using StarterAssets;
using UnityEngine.InputSystem;
using Cinemachine;

public class checkLocalPlayer : NetworkBehaviour
{
    //enable these transforms
    public Transform[] EnableTransforms;
    [SerializeField] Transform[] EnableMobileTransforms;
    public bool isVr;
    public bool assignedAsLocalPlayer = false;


[Header("leave empty if NOT IN VR ")]
    [SerializeField] XRRig XR_Rig;
    [SerializeField] ActionBasedController ActionControllerLeft;
    [SerializeField] ActionBasedController ActionControllerRight;
    [SerializeField] InputActionManager inputActionManager;
    [SerializeField] XRDirectInteractor xRDirectInteractorLeft;
    [SerializeField] XRDirectInteractor xRDirectInteractorRight;


    [Header("leave empty if IN VR ")]
    [SerializeField] RigidbodyThirdPersonController thirdPersonController;
    [SerializeField] PlayerInput playerInput;

   
    //this only runes if the object it is on is the local player, so we enable all the controlls and cammeras here
    public void Start ()
    {

        NetworkIdentity netID = GetComponent<NetworkIdentity>();

        if(netID.hasAuthority){
            assignedAsLocalPlayer = true;
        }

        if (assignedAsLocalPlayer)
        {

            if (!isVr)
            {
                thirdPersonController.enabled = true;
                playerInput.enabled = true;
            }

            for (int i = 0; i < EnableTransforms.Length; i++)
            {
                EnableTransforms[i].gameObject.SetActive(true);
            }

            #if UNITY_ANDROID

            for (int i = 0; i < EnableMobileTransforms.Length; i++)
            {
                EnableMobileTransforms[i].gameObject.SetActive(true);
            }
            #endif

            if (!isVr)
            {
                GameObject Camera = GameObject.Find("Camera");
                Camera.GetComponentInChildren<CinemachineVirtualCamera>().Follow = this.transform;
                //removes the VR settings if the local player is not a vr player
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();
                print("Turning of Vr");
                print(GetComponent<NetworkIdentity>().netId);
                print(gameObject.name);
            }
            else
            {
                print("initializing VR");
                XRGeneralSettings.Instance.Manager.InitializeLoader();
                XR_Rig.enabled = true;
                ActionControllerLeft.enabled = true;
                ActionControllerRight.enabled = true;
                inputActionManager.enabled = true;
                xRDirectInteractorLeft.enabled = true;
                xRDirectInteractorRight.enabled = true;
            }
        }
       
        
    }

}
