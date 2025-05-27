using System.Collections;
using UnityEngine;

public class ClientStateLeave<T> : State<T>
{
    private ClientModel clientModel;
    private ClientView clientView;
    private Transform newTransform;

    private bool canLeave = false;

    public bool CanLeave { get => canLeave; set => canLeave = value; }


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

        // Quiere decir que entro al estado leave porque todas las mesas estaban ocupadas
        if (clientModel.CurrentTablePosition == null)
        {
            clientView.ExecuteAnimParameterName("Walk");
            clientModel.MoveToTarget(newTransform.position);
            clientModel.LookAt(newTransform.position, clientView.Anim.transform);
        }

        // Sino quiere decir que se fue de leave porque estaba ya en una mesa
        else
        {
            clientView.ExecuteAnimParameterName("StandUp");
            clientView.Anim.transform.position += Vector3.up * 0.35f;
            clientView.StartCoroutine(WalkAnimationAfterExitTime());
        }

        clientModel.ClientManager.FreeTable(clientModel.CurrentTablePosition);

        /// provisorio preguntar al profesor
        clientModel.CurrentTablePosition = null;
    }

    public override void Execute()
    {
        base.Execute();
    }

    public override void Exit()
    {
        base.Exit();

        canLeave = false;
    }


    private IEnumerator WalkAnimationAfterExitTime()
    {
        yield return new WaitForSeconds(3f);// Tiempo que tarda en pararse por completo 

        clientView.Anim.transform.position += Vector3.down * 0.35f;
        clientModel.MoveToTarget(newTransform.position);
        clientModel.LookAt(newTransform.position, clientView.Anim.transform);
        clientView.ExecuteAnimParameterName("Walk");
    }
}
