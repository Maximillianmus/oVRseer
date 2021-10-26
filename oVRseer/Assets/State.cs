using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public enum PlayerState
{
    Inside,
    Outside,
    Squashed,
}


public class State : NetworkBehaviour
{
    [SyncVar(hook =  "OnPlayerChangeState")]
    [SerializeField] private PlayerState state;

    public PlayerState stateProp
    {
        get => state;
        set => state = value;
    }


    [SerializeField] private UnityEvent onPlayerDeadNotifyServer;
    
    public bool isPlaying()
    {
        return state == PlayerState.Inside;
    }

    public void Outside()
    {
        if (!hasAuthority)
            return;
        CmdPlayerOutside();
    }


    [Command]
    public void CmdPlayerOutside()
    {
        state = PlayerState.Outside;
    }

    public void Squashed()
    {
        if (!hasAuthority)
            return;
        CmdPlayerSquashed();
    }

    [Command]
    public void CmdPlayerSquashed()
    {
        state = PlayerState.Squashed;
    }

    private void OnPlayerChangeState(PlayerState oldState, PlayerState newState)
    {
        if (oldState == newState || !hasAuthority)
        {
            return;
        }

        if (newState == PlayerState.Inside)
        {
            throw new ArgumentException("It is impossible to revive");
        }
        
        // ChangeUI
        if (newState == PlayerState.Outside)
            gameObject.GetComponent<Despawn>().Win();
        if (newState == PlayerState.Squashed)
            gameObject.GetComponent<Despawn>().Kill();

        onPlayerDeadNotifyServer.Invoke();
    }

}
