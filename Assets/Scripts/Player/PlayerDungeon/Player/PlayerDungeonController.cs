using UnityEngine;

[RequireComponent(typeof(PlayerDungeonModel), typeof(PlayerDungeonView))]
public class PlayerDungeonController : MonoBehaviour
{
    private PlayerDungeonModel model;
    private PlayerDungeonView view;

    private CombatHandler combat;
    private DashHandler dash;

    private FSM<PlayerPhase> fsm = new FSM<PlayerPhase>();

    private void Awake()
    {
        GetReferences();
        InitializeFSM();
    }

    private void Update()
    {
        if (model.IsDead) return;
        fsm.OnExecute();
        
    }

    private void GetReferences()
    {
        model = GetComponent<PlayerDungeonModel>();
        view = GetComponent<PlayerDungeonView>();
        combat = GetComponent<CombatHandler>();
        dash = GetComponent<DashHandler>();
    }

    private void InitializeFSM()
    {
        var stateIdle = new PlayerStateDungeonIdle<PlayerPhase>(
      model, view,
      PlayerPhase.Walk, PlayerPhase.Jump, PlayerPhase.Combat, PlayerPhase.Dashing
  );

        var stateWalk = new PlayerStateDungeonWalk<PlayerPhase>(
            model, view,
            PlayerPhase.Idle, PlayerPhase.Run, PlayerPhase.Jump, PlayerPhase.Combat, PlayerPhase.Dashing
        );

        var stateRun = new PlayerStateDungeonRun<PlayerPhase>(
            model, view,
            PlayerPhase.Idle, PlayerPhase.Walk, PlayerPhase.Jump
        );

        var stateJump = new PlayerStateDungeonJump<PlayerPhase>(
            model, view,
            PlayerPhase.Idle
        );

        var stateCombat = new PlayerStateDungeonCombat<PlayerPhase>(
            model, view,
            combat, PlayerPhase.Idle
        );

        var stateDash = new PlayerStateDungeonDash<PlayerPhase>(
            model, view,
            dash, PlayerPhase.Idle
        );

        var stateDead = new PlayerStateDungeonDead<PlayerPhase>(
            model, view
        );

        stateIdle.AddTransition(PlayerPhase.Walk, stateWalk);
        stateIdle.AddTransition(PlayerPhase.Jump, stateJump);
        stateIdle.AddTransition(PlayerPhase.Combat, stateCombat);
        stateIdle.AddTransition(PlayerPhase.Dashing, stateDash);
        stateIdle.AddTransition(PlayerPhase.Dead, stateDead);

        stateWalk.AddTransition(PlayerPhase.Idle, stateIdle);
        stateWalk.AddTransition(PlayerPhase.Run, stateRun);
        stateWalk.AddTransition(PlayerPhase.Jump, stateJump);
        stateWalk.AddTransition(PlayerPhase.Combat, stateCombat);
        stateWalk.AddTransition(PlayerPhase.Dashing, stateDash);
        stateWalk.AddTransition(PlayerPhase.Dead, stateDead);

        stateRun.AddTransition(PlayerPhase.Idle, stateIdle);
        stateRun.AddTransition(PlayerPhase.Walk, stateWalk);
        stateRun.AddTransition(PlayerPhase.Jump, stateJump);
        stateRun.AddTransition(PlayerPhase.Combat, stateCombat);
        stateRun.AddTransition(PlayerPhase.Dashing, stateDash);
        stateRun.AddTransition(PlayerPhase.Dead, stateDead);

        stateJump.AddTransition(PlayerPhase.Idle, stateIdle);
        stateJump.AddTransition(PlayerPhase.Dead, stateDead);

        stateCombat.AddTransition(PlayerPhase.Idle, stateIdle);
        stateCombat.AddTransition(PlayerPhase.Dead, stateDead);

        stateDash.AddTransition(PlayerPhase.Idle, stateIdle);
        stateDash.AddTransition(PlayerPhase.Dead, stateDead);

        stateDead.AddTransition(PlayerPhase.Idle, stateIdle); // Optional respawn path

        fsm.SetInit(stateIdle);
    }
}
