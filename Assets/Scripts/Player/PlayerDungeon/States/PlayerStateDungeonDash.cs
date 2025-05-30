using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDungeonDash<T> : State<T>
{
    private PlayerDungeonModel model;
    private PlayerDungeonView view;
    private DashHandler dash;
    private T idle;

    private float dashStartTime;
    private float dashDuration = 0.3f;

    public PlayerStateDungeonDash(PlayerDungeonModel model, PlayerDungeonView view, DashHandler dash, T idle)
    {
        this.model = model;
        this.view = view;
        this.dash = dash;
        this.idle = idle;
    }

    public override void Enter()
    {
        base.Enter();

        if (model.IsDead)
        {
            Fsm.TransitionTo((T)(object)PlayerPhase.Dead);
            return;
        }

        dash.ExecuteDash();
        dashStartTime = Time.time;
    }

    public override void Execute()
    {
        base.Execute();

        if (model.IsDead)
        {
            Fsm.TransitionTo((T)(object)PlayerPhase.Dead);
            return;
        }

        if (Time.time >= dashStartTime + dashDuration)
        {
            Fsm.TransitionTo(idle);
        }
    }

    public override void Exit()
    {
        base.Exit();
        // Clear invulnerability for safety (even if DashHandler already handles it)
        model.SetInvulnerable(false);
    }
}
