using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDungeonCombat<T> : State<T>
{
    private PlayerDungeonModel model;
    private PlayerDungeonView view;
    private CombatHandler combat;
    private T idle;

    private float attackStartTime;
    private float attackDuration = 0.5f; // Sync with animation duration if needed

    public PlayerStateDungeonCombat(PlayerDungeonModel model, PlayerDungeonView view, CombatHandler combat, T idle)
    {
        this.model = model;
        this.view = view;
        this.combat = combat;
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

        combat.TryAttack();
        attackStartTime = Time.time;

    }
    public override void Execute()
    {
        base.Execute();

        // If player dies during combat
        if (model.IsDead)
        {
            Fsm.TransitionTo((T)(object)PlayerPhase.Dead);
            return;
        }

        // Transition back to idle after attack cooldown
        if (Time.time >= attackStartTime + attackDuration)
        {
            Fsm.TransitionTo(idle);
        }
    }
    public override void Exit()
    {
        base.Exit();
        // Any cleanup or animation reset if necessary
    }
}
