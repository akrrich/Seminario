using UnityEngine;

public class RatEnemyController : AnimalEnemyController
{
    private RatEnemyModel ratModel;

    protected override void GetComponents()
    {
        base.GetComponents();
        ratModel = GetComponent<RatEnemyModel>();
    }

    protected override void InitializeFSM()
    {
        
        var idle = new EnemyStateIdle<EnemyStates>(ratModel, view); // o AnimalStateIdle si querés
        var chase = new RatStateChase<EnemyStates>(ratModel, view);
        var attack = new EnemyStateAttack<EnemyStates>(ratModel, view, this);
        var dead = new EnemyStateDead<EnemyStates>(ratModel, view);

        idle.AddTransition(EnemyStates.Chase, chase);
        chase.AddTransition(EnemyStates.Attack, attack);
        attack.AddTransition(EnemyStates.Chase, chase);
        attack.AddTransition(EnemyStates.Dead, dead);

        fsm.SetInit(idle);
    }
}