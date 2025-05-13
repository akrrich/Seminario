using UnityEngine;

public class ClientStateLeave<T> : State<T>
{
    private ClientModel clientModel;
    private ClientView clientView;
    private Transform newTransform;


    public ClientStateLeave(ClientModel clientModel, ClientView clientView, Transform newTransform)
    {
        this.clientModel = clientModel;
        this.clientView = clientView;
        this.newTransform = newTransform;
    }


    public override void Enter()
    {
        base.Enter();
        Debug.Log("Leave");

        clientModel.MoveToTarget(newTransform);
        //clientView.StandUpAnim();
    }

    public override void Execute()
    {
        base.Execute();
    }
}
