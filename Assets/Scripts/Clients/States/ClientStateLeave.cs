using System.Collections;
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

        clientView.StandUpAnim();
        clientView.Anim.transform.position += Vector3.up * 0.35f;
        clientView.StartCoroutine(WalkAnimationAfterExitTime());
    }

    public override void Execute()
    {
        base.Execute();
    }


    private IEnumerator WalkAnimationAfterExitTime()
    {
        yield return new WaitForSeconds(3f);// Tiempo que tarda en pararse por completo 

        clientView.Anim.transform.position += Vector3.down * 0.35f;
        clientModel.MoveToTarget(newTransform);
        clientModel.LookAt(newTransform);
        clientView.WalkAnim();
    }
}
