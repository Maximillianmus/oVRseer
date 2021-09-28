using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionCD : MonoBehaviour
{
    private bool OnCd;
    private float beginTime;
    private float cooldown;
    public GameObject CDBackground;
    public Text numberOfSeconds;


    // Update is called once per frame
    void Update()
    {
        if (OnCd)
        {
            if (Time.time - beginTime < cooldown)
            {
                numberOfSeconds.text = (cooldown - (Time.time - beginTime)).ToString();
                CDBackground.SetActive(true);
            }
            else
            {
                OnCd = false;
                CDBackground.SetActive(false);
            }
        }
        else
        {
            CDBackground.SetActive(false);
        }
    }

    public void OnActionPress(float beginTime, float cooldown)
    {
        this.beginTime = beginTime;
        this.cooldown = cooldown;
        OnCd = true;
    }
}
