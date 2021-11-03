using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SwitchMaterial : XRBaseInteractable
{
    public Material newmaterial;
    

    //Get the GameObjects mesh renderer to access the GameObjects material and color
    MeshRenderer m_Renderer;

    Material oldMaterial;


    void Start()
    {
        //Fetch the mesh renderer component from the GameObject
        m_Renderer = GetComponent<MeshRenderer>();
        oldMaterial = m_Renderer.material;
        
    }

    /*void OnMouseOver()
    {
        // Set the new material on the GameObject
        m_Renderer.material = newmaterial;
    }


    void OnMouseExit()
    {
        m_Renderer.material = oldMaterial;
    }*/

    protected virtual void OnHoverEnter(XRBaseInteractor interactor)
    {
        m_Renderer.material = newmaterial;
    }

    protected virtual void OnHoverExit(XRBaseInteractor interactor)
    {
        m_Renderer.material = oldMaterial;
    }
}
