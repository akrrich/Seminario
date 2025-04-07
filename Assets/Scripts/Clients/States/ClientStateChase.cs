using UnityEngine;

public class ClientStateChase<T> : State<T>
{
    private ClientModel clientModel;
    private Transform newTransform;


    public ClientStateChase(ClientModel clientModel, Transform newTransform)
    {
        this.clientModel = clientModel;
        this.newTransform = newTransform;
    }


    public override void Execute()
    {
        base.Execute();
        Debug.Log("Chasing" + newTransform);

        clientModel.MoveToTarget(newTransform);
    }
}
