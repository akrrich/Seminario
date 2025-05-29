using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RatStateChase<T> : State<T>
{
    private RatEnemyModel model;
    private BaseEnemyView view;
    private float changeTargetInterval = 0.5f;
    private float timer;

    public RatStateChase(RatEnemyModel model, BaseEnemyView view)
    {
        this.model = model;
        this.view = view;
    }

    public override void Enter()
    {
        timer = 0f;
        view.PlayMoveAnimation(true);
    }

    public override void Execute()
    {
        timer += Time.deltaTime;
        if (timer >= changeTargetInterval)
        {
            timer = 0f;
            Vector3 newTarget = model.GetRandomCircleTargetAroundPlayer();
            model.MoveTo(newTarget);
            model.LookAt(model.Player.transform.position);
        }
    }

    public override void Exit()
    {
        view.PlayMoveAnimation(false);
    }
}
