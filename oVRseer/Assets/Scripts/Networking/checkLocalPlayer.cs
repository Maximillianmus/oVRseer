using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.XR.Management;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using StarterAssets;

public class checkLocalPlayer : NetworkBehaviour
{
    //enable these transforms
    public Transform[] EnableTransforms;
    public bool isVr;

    [Header("leave empty if NOT IN VR ")]
    [SerializeField] XRRig XR_Rig;
    [SerializeField] ActionBasedController ActionControllerLeft;
    [SerializeField] XRRayInteractor XRRayInteractorLeft;
    [SerializeField] ActionBasedController ActionControllerRight;
    [SerializeField] XRRayInteractor XRRayInteractorRight;
    [SerializeField] InputActionManager inputActionManager;


    [Header("leave empty if IN VR ")]
    [SerializeField] CharacterController characterController;
    [SerializeField] ThirdPersonController thirdPersonController;



    //this only runes if the object it is on is the local player, so we enable all the controlls and cammeras here
    public override void OnStartLocalPlayer()
    {

        NetworkIdentity netID = GetComponent<NetworkIdentity>();

        Debug.LogWarning("The code in the check loader is currently using a hack to circumvent a bug, please fix bug and then remove circumvention. Bug can be found on git");
        if (netID.isLocalPlayer && netID.netId != 3 && netID.netId != 4)
        {

            if (!isVr)
            {
                characterController.enabled = true;
                thirdPersonController.enabled = true;
            }

            for (int i = 0; i < EnableTransforms.Length; i++)
            {
                EnableTransforms[i].gameObject.SetActive(true);
            }

            if (!isVr)
            {
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
                XRRayInteractorLeft.enabled = true;
                ActionControllerRight.enabled = true;
                XRRayInteractorRight.enabled = true;
                inputActionManager.enabled = true;
            }
        }
       
        
    }

}
