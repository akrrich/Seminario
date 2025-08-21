using UnityEngine;

public class ClientStateIdle<T> : State<T>
{
    private ClientModel clientModel;
    private ClientView clientView;


    public ClientStateIdle(ClientModel clientModel, ClientView clientView)
    {
        this.clientModel = clientModel;
        this.clientView = clientView;
    }


    public override void Enter()
    {
        base.Enter();
        Debug.Log("IdleClient");

        clientModel.StopVelocity();
        clientModel.ClientManager.ReturnObjectToPool(clientModel.ClientType, clientModel);
        clientView.DisableAllSpriteTypes();
        clientView.Anim.transform.position = clientModel.transform.position;
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
