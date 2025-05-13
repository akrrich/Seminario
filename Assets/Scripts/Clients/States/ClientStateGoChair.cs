using System;
using UnityEngine;

public class ClientStateGoChair<T> : State<T>
{
    private ClientModel clientModel;
    private ClientView clientView;
    private Func<Transform> getTargetTransform; 


    public ClientStateGoChair(ClientModel clientModel, ClientView clientView, Func<Transform> getTargetTransform)
    {
        this.clientModel = clientModel;
        this.clientView = clientView;
        this.getTargetTransform = getTargetTransform;
    }


    public override void Enter()
    {
        base.Enter();
        Debug.Log("GoChair");

        clientModel.MoveToTarget(getTargetTransform());
        //clientView.WalkAnim();
    }

    public override void Execute()
    {
        base.Execute();
    }
}
