using UnityEngine;

public class ClientStateIdle<T> : State<T>
{
    private ClientModel clientModel;
    private MonoBehaviour mono;


    public ClientStateIdle(ClientModel clientModel)
    {
        this.clientModel = clientModel;
    }


    public override void Enter()
    {
        base.Enter();
        Debug.Log("IdleClient");

        clientModel.StopVelocity();
    }

    public override void Execute()
    {
        base.Execute();
    }
}
