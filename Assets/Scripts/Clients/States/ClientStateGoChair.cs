using System;
using UnityEngine;

public class ClientStateGoChair<T> : State<T>
{
    private ClientModel clientModel;
    private Func<Transform> getTargetTransform; 


    public ClientStateGoChair(ClientModel clientModel, Func<Transform> getTargetTransform)
    {
        this.clientModel = clientModel;
        this.getTargetTransform = getTargetTransform;
    }


    public override void Enter()
    {
        base.Enter();
        Debug.Log("GoChair");

        clientModel.MoveToTarget(getTargetTransform());
    }

    public override void Execute()
    {
        base.Execute();
    }
}
