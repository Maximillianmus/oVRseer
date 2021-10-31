using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateMovementTiny
{
    Idle,
    Walk,
    Run,
    Jump,
    Landing
}

public class StepSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip runClip;
    [SerializeField] private AudioClip landingClip;
    [SerializeField] private AudioClip idleClip;

    private StateMovementTiny state;
    private bool paused = false;


    private static float runThresh = 4.0f;
    private static float walkThresh = 1.0f;

    private int _speedId;
    private int _jumpId;
    private int _groundedId;

    private void Start()
    {
        _speedId = Animator.StringToHash("Speed");
        _jumpId = Animator.StringToHash("Jump");
        _groundedId = Animator.StringToHash("Grounded");
        state = StateMovementTiny.Idle;
    }


    // Update is called once per frame
    void Update()
    {
        var speed = animator.GetFloat(_speedId);
        var jump = animator.GetBool(_jumpId);
        var grounded = animator.GetBool(_groundedId);
        StateMovementTiny newState = StateMovementTiny.Idle;
        if (speed >= runThresh && grounded)
        {
            newState = StateMovementTiny.Run;
        } else if (speed >= walkThresh && grounded)
        {
            newState = StateMovementTiny.Walk;
        } 
        if (state == StateMovementTiny.Jump && !jump)
        {
            newState = StateMovementTiny.Landing;
        }

        if (jump)
        {
            newState = StateMovementTiny.Jump;
        }

        if (newState != state)
        {
            UpdateState(newState);
        }
    }

    private void UpdateState(StateMovementTiny newState)
    {
        print(state + " " + newState);
        switch (newState)
        {
            case StateMovementTiny.Idle:
                PlayStateAudio(idleClip);
                break;
            case StateMovementTiny.Walk:
                PlayStateAudio(walkClip);
                break;
            case StateMovementTiny.Run:
                PlayStateAudio(runClip);
                break;
            case StateMovementTiny.Jump:
                audioSource.Pause();
                paused = true;
                break;
            case StateMovementTiny.Landing:
                audioSource.PlayOneShot(landingClip);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        state = newState;
    }

    private void PlayStateAudio(AudioClip clip)
    {
        if (paused && audioSource.clip == clip)
        {
            audioSource.UnPause();
            paused = false;
        }
        else
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
