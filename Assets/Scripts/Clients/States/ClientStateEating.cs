using UnityEngine;

public class ClientStateEating<T> : State<T>
{
    private ClientModel clientModel;
    private ClientView clientView;


    public ClientStateEating(ClientModel clientModel, ClientView clientView)
    {
        this.clientModel = clientModel;
        this.clientView = clientView;
    }


    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        base.Execute();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
