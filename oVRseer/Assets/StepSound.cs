using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateMovementTiny
{
    Idle,
    Walk,
    Run
}

public class StepSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip runClip;

    private StateMovementTiny state;
    private bool paused = false;

    private static float runThresh = 4.0f;
    private static float walkThresh = 1.0f;

    private int _speedId;

    private void Start()
    {
        _speedId = Animator.StringToHash("Speed");
        state = StateMovementTiny.Idle;
    }


    // Update is called once per frame
    void Update()
    {
        var speed = animator.GetFloat(_speedId);
        StateMovementTiny newState = StateMovementTiny.Idle;
        if (speed >= runThresh)
        {
            newState = StateMovementTiny.Run;
        } else if (speed >= walkThresh)
        {
            newState = StateMovementTiny.Walk;
        }

        if (newState != state)
        {
            UpdateState(newState);
        }
    }

    private int justPause(StateMovementTiny oldState, StateMovementTiny newState)
    {
        if (oldState == StateMovementTiny.Idle && newState != StateMovementTiny.Idle)
        {
            return 1;
        } else if (oldState != StateMovementTiny.Idle && newState == StateMovementTiny.Idle)
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }

    private void UpdateState(StateMovementTiny newState)
    {
        switch (justPause(state, newState))
        {
            case 1:
                if (paused)
                {
                    audioSource.UnPause();
                    paused = false;
                }
                else
                    audioSource.Play();
                break;
            case 0:
                audioSource.Pause();
                paused = true;
                break;
            case -1:
                if (newState == StateMovementTiny.Run)
                {
                    audioSource.clip = runClip;
                    audioSource.Play();
                }
                else
                {
                    audioSource.clip = walkClip;
                    audioSource.Play();
                }

                break;
            default:
                break;

        }
        state = newState;
    }
}
