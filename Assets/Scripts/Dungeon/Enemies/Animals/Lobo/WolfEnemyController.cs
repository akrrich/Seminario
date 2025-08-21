
using UnityEngine;

public class WolfEnemyController : AnimalEnemyController
{
    protected override bool QuestionIsInAttackRange()
    {
        float dist = Vector3.Distance(transform.position, model.Player.transform.position);
        return dist <= 5f;
    }

    public override void Attack()
    {
        base.Attack();
        view.PlayAttackAnimation(); // Se puede sobreescribir con salto
    }
}
