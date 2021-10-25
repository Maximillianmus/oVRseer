using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;

    private int _speedId;

    private void Start()
    {
        _speedId = Animator.StringToHash("Speed");

    }


    // Update is called once per frame
    void Update()
    {
        var speed = animator.GetFloat(_speedId);
        if (speed >= 1.0f)
        {
            audioSource.UnPause();
        }
        else
        {
            audioSource.Pause();
        }
    }
}
