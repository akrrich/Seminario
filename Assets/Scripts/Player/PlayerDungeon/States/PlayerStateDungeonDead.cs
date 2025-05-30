using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDungeonDead<T> : State<T>
{
    private PlayerDungeonModel model;
    private PlayerDungeonView view;

    public PlayerStateDungeonDead(PlayerDungeonModel model, PlayerDungeonView view)
    {
        this.model = model;
        this.view = view;
    }
    public override void Enter()
    {
        base.Enter();
        model.RequestPhaseChange(PlayerPhase.Dead);
        model.Speed = 0f;
        model.MoveDirection = Vector3.zero;

        // Play death animation if available
        view.PlayDeathAnimation();

        // Optional: disable collider, UI, etc.
        model.Rb.velocity = Vector3.zero;
        
    }
}
