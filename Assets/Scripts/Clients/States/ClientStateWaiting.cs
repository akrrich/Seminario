using UnityEngine;

public class ClientStateWaiting<T> : State<T>
{
    private ClientModel clientModel;
    private ClientView clientView;


    public ClientStateWaiting(ClientModel clientModel, ClientView clientView)
    {
        this.clientModel = clientModel;
        this.clientView = clientView;
    }


    public override void Enter()
    {
        base.Enter();
        Debug.Log("Waiting");

        clientModel.StopVelocity();
        //clientView.SitAnim();
        ClientModel.OnWaitingFood?.Invoke();
    }

    public override void Execute()
    {
        base.Execute();
    }
}
