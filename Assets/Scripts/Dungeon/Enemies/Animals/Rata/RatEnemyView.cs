using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatEnemyView : AnimalEnemyView
{
    public override void PlayMoveAnimation(bool isMoving)
    {
        base.PlayMoveAnimation(isMoving);
        Debug.Log("RatEnemyView: corretea de forma errática.");
    }

    public override void PlayAttackAnimation()
    {
        base.PlayAttackAnimation();
        Debug.Log("RatEnemyView: animación de mordida rápida.");
    }

    public override void PlayDeathAnimation()
    {
        base.PlayDeathAnimation();
        Debug.Log("RatEnemyView: chillido de muerte.");
    }
}
