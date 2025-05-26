using System;
using UnityEngine;

public class ClientStateGoChair<T> : State<T>
{
    private ClientModel clientModel;
    private ClientView clientView;
    private Func<Transform> getTargetTransform;

    //private Seek seek;


    public ClientStateGoChair(ClientModel clientModel, ClientView clientView, Func<Transform> getTargetTransform)
    {
        this.clientModel = clientModel;
        this.clientView = clientView;
        this.getTargetTransform = getTargetTransform;

        //seek = new Seek(clientModel.transform, null); 
    }


    public override void Enter()
    {
        base.Enter();
        Debug.Log("GoChair");

        //clientModel.CurrentTablePosition.ChairPosition.gameObject.layer = LayerMask.NameToLayer("Default");

        //Transform target = getTargetTransform();
        //seek.SetTarget(target);

        clientModel.MoveToTarget(getTargetTransform().position);
        clientModel.LookAt(getTargetTransform().position, clientView.Anim.transform);
        clientView.ExecuteAnimParameterName("Walk");
        clientView.SetSpriteTypeName("SpriteGoChair");
    }

    public override void Execute()
    {
        base.Execute();

        /*Vector3 dir = seek.GetDir();
        Vector3 finalDir = clientModel.ObstacleAvoidance.GetDir(dir);

        Vector3 lookCurrentTransform = clientView.transform.position + finalDir;

        clientModel.LookAt(lookCurrentTransform, clientView.Anim.rootRotation);
        clientModel.MoveToTarget(finalDir);*/
    }

    public override void Exit() 
    { 
        base.Exit();

        //clientModel.CurrentTablePosition.ChairPosition.gameObject.layer = LayerMask.NameToLayer("Obstacles");
    }
}
