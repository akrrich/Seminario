public class EnemyStateDead<T> : State<T>
{
    private BaseEnemyModel model;
    private BaseEnemyView view;

    public EnemyStateDead(BaseEnemyModel model, BaseEnemyView view)
    {
        this.model = model;
        this.view = view;
    }

    public override void Enter()
    {
        view.PlayDeathAnimation();
        model.Stop();
        model.Agent.enabled = false;
    }

    public override void Execute() { }
    public override void Exit() { }
}
