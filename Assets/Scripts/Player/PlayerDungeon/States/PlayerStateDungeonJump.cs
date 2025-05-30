using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDungeonJump<T> : State<T>
{
    private PlayerDungeonModel model;
    private PlayerDungeonView view;
    private T idleInput;
    private bool hasJumped;

    public PlayerStateDungeonJump(PlayerDungeonModel model, PlayerDungeonView view, T idleInput)
    {
        this.model = model;
        this.view = view;
        this.idleInput = idleInput;
    }
    public override void Enter()
    {
        base.Enter();
        hasJumped = false;

        if (model.IsGrounded)
        {
            model.Jump();
         //   view.PlayJumpAnimation();
            hasJumped = true;
        }
    }

    public override void Execute()
    {
        base.Execute();

        if (model.IsGrounded && hasJumped)
        {
            Fsm.TransitionTo(idleInput);
        }
    }

    public override void Exit()
    {
        base.Exit();
        // Optional: cleanup or reset jump state
    }
}
