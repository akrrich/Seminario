public class EnemyStateChase<T> : State<T>
{
    private BaseEnemyModel model;
    private BaseEnemyView view;

    public EnemyStateChase(BaseEnemyModel model, BaseEnemyView view)
    {
        this.model = model;
        this.view = view;
    }

    public override void Enter()
    {
        view.PlayMoveAnimation(true);
    }

    public override void Execute()
    {
        model.MoveTo(model.Player.transform.position);
        model.LookAt(model.Player.transform.position);
    }

    public override void Exit()
    {
        model.Stop();
        view.PlayMoveAnimation(false);
    }
}
