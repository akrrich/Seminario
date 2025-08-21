public class EnemyStateIdle<T> : State<T>
{
    private BaseEnemyModel model;
    private BaseEnemyView view;

    public EnemyStateIdle(BaseEnemyModel model, BaseEnemyView view)
    {
        this.model = model;
        this.view = view;
    }

    public override void Enter()
    {
        model.Stop();
        view.PlayMoveAnimation(false);
    }

    public override void Execute() { }
    public override void Exit() { }
}
