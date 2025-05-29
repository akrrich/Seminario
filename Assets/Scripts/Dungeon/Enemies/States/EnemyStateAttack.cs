using UnityEngine;
public class EnemyStateAttack<T> : State<T>
{
    private BaseEnemyModel model;
    private BaseEnemyView view;
    private IAttackable attackable;

    private float attackTimer = 0f;

    public EnemyStateAttack(BaseEnemyModel model, BaseEnemyView view, IAttackable attackable)
    {
        this.model = model;
        this.view = view;
        this.attackable = attackable;
    }

    public override void Enter()
    {
        attackTimer = 0f;
        model.Stop();
        view.PlayMoveAnimation(false);
    }

    public override void Execute()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= model.AttackCooldown)
        {
            attackable.Attack();
            attackTimer = 0f;
        }

        model.LookAt(model.Player.transform.position);
    }

    public override void Exit() { }
}
