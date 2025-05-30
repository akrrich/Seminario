using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDungeonIdle<T> : State<T>
{
    private PlayerDungeonModel dungeonModel;
    private PlayerDungeonView dungeonView;
    private T walkInput;
    private T jumpInput;
    private T combatInput;
    private T dashInput;

    public PlayerStateDungeonIdle(PlayerDungeonModel dungeonModel, PlayerDungeonView dungeonView, T walkInput, T jumpInput, T combatInput, T dashInput)
    {
        this.dungeonModel = dungeonModel;
        this.dungeonView = dungeonView;
        this.walkInput = walkInput;
        this.jumpInput = jumpInput;
        this.combatInput = combatInput;
        this.dashInput = dashInput;
    }
    public override void Enter()
    {
        base.Enter();
        //view animation
    }
    public override void Execute()
    {
        base.Execute();
        // === Transition Conditions ===
        if (PlayerInputs.Instance == null) return;

        if (dungeonModel.IsDead)
        {
            Fsm.TransitionTo((T)(object)PlayerPhase.Dead);
            return;
        }

        if (PlayerInputs.Instance.Dash() && dungeonModel.CanDash)
        {
            Fsm.TransitionTo(dashInput);
            return;
        }

        if (PlayerInputs.Instance.Attack())
        {
            Fsm.TransitionTo(combatInput);
            return;
        }

        if (PlayerInputs.Instance.Jump() && dungeonModel.IsGrounded)
        {
            Fsm.TransitionTo(jumpInput);
            return;
        }
        var movement = PlayerInputs.Instance?.GetMoveAxis() ?? Vector2.zero;
        if (movement.magnitude > 0.1f)
        {
            Fsm.TransitionTo(walkInput);
            return;
        }
    }
    public override void Exit()
    {
        base.Exit();
        // Optional: Cleanup or stop animations
    }
}
