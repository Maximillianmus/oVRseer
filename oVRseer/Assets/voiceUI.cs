using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class voiceUI : MonoBehaviour
{
    [SerializeField] private Image micOffImg;
    [SerializeField] private Image micOnImg;
    

    
    public void OnVoiceOn()
    {
        micOffImg.gameObject.SetActive(false);
        micOnImg.gameObject.SetActive(true);
    }
    
    public void OnVoiceOff()
    {
        micOffImg.gameObject.SetActive(true);
        micOnImg.gameObject.SetActive(false);
    }
}
