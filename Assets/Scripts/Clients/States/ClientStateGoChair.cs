using UnityEngine;

public class ClientStateGoChair<T> : State<T>
{
    private ClientModel clientModel;
    private Transform newTransform;


    public ClientStateGoChair(ClientModel clientModel, Transform newTransform)
    {
        this.clientModel = clientModel;
        this.newTransform = newTransform;
    }


    public override void Enter()
    {
        base.Enter();
        Debug.Log("GoChair");

        clientModel.MoveToTarget(newTransform);
    }

    public override void Execute()
    {
        base.Execute();
    }
}
