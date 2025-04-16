using UnityEngine;

public class ClientStateWaiting<T> : State<T>
{
    private ClientModel clientModel;


    public ClientStateWaiting(ClientModel clientModel)
    {
        this.clientModel = clientModel;
    }


    public override void Enter()
    {
        base.Enter();
        Debug.Log("Waiting");

        clientModel.StopVelocity();
        ClientModel.OnWaitingFood?.Invoke();
    }

    public override void Execute()
    {
        base.Execute();
    }
}
