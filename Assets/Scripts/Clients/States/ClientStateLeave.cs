using UnityEngine;

public class ClientStateLeave<T> : State<T>
{
    private ClientModel clientModel;
    private Transform newTransform;


    public ClientStateLeave(ClientModel clientModel, Transform newTransform)
    {
        this.clientModel = clientModel;
        this.newTransform = newTransform;
    }


    public override void Enter()
    {
        base.Enter();
        Debug.Log("Leave");

        clientModel.MoveToTarget(newTransform);
    }

    public override void Execute()
    {
        base.Execute();
    }
}
