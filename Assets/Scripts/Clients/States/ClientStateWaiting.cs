using System.Collections;
using UnityEngine;

public class ClientStateWaiting<T> : State<T>
{
    private ClientModel clientModel;
    private ClientView clientView;


    public ClientStateWaiting(ClientModel clientModel, ClientView clientView)
    {
        this.clientModel = clientModel;
        this.clientView = clientView;
    }


    public override void Enter()
    {
        base.Enter();
        Debug.Log("Waiting");

        clientModel.StopVelocity();
        clientModel.LookAt(clientModel.CurrentTablePosition.transform);
        clientView.SitAnim();
        clientView.StartCoroutine(DuringSitAnimationAfterExitTime());
    }

    public override void Execute()
    {
        base.Execute();
    }

    public override void Exit()
    {
        base.Exit();

        clientView.Anim.transform.position += Vector3.down * 0.38f;
    }


    private IEnumerator DuringSitAnimationAfterExitTime()
    {
        yield return new WaitForSeconds(4.12f); // Tiempo que tarda en sentarse por completo
        
        clientView.DuringSit();
        clientView.Anim.transform.position += Vector3.up * 0.38f;
    }
}
