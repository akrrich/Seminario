using System;
using UnityEngine;

public class ClientStateGoChair<T> : State<T>
{
    private ClientModel clientModel;
    private ClientView clientView;
    private Func<Transform> getTargetTransform;

    private float distanceToChair = 6f;


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

        clientModel.MoveToTarget(getTargetTransform().position);
        clientModel.LookAt(getTargetTransform().position, clientView.Anim.transform);
        clientView.ExecuteAnimParameterName("Walk");
        clientView.SetSpriteTypeName("SpriteGoChair");
    }

    public override void Execute()
    {
        base.Execute();

        CheckDistanceFromTransformToCurrentChair();
    }

    public override void Exit() 
    { 
        base.Exit();
    }


    private void CheckDistanceFromTransformToCurrentChair()
    {
        if (Vector3.Distance(clientModel.transform.position, clientModel.CurrentTable.ChairPosition.position) <= distanceToChair)
        {
            clientModel.CurrentTable.SetNavMeshObstacles(false);
        }
    }
}
