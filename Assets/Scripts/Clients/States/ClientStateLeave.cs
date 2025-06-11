using System.Collections;
using UnityEngine;

public class ClientStateLeave<T> : State<T>
{
    private ClientModel clientModel;
    private ClientView clientView;
    private ClientController clientController;
    private Transform newTransform;

    private float waitingTimeToFreeTable = 6f;

    private bool canLeave = false;

    public bool CanLeave { get => canLeave; set => canLeave = value; }


    public ClientStateLeave(ClientController clientController, ClientModel clientModel, ClientView clientView, Transform newTransform)
    {
        this.clientController = clientController;
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

        clientModel.StartCoroutine(FreeCurrentTableAfterSeconds());
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

        yield return clientController.StartCoroutine(EnabledTriggerFromTable());
    }

    private IEnumerator EnabledTriggerFromTable()
    {
        /// Ajustar tiempo segun sea necesario
        yield return new WaitForSeconds(2f);

        clientController.OnCollisionEnterWithTrigger = false;
    }

    private IEnumerator FreeCurrentTableAfterSeconds()
    {
        yield return new WaitForSeconds(waitingTimeToFreeTable);

        clientModel.CurrentTablePosition.SetNavMeshObstacles(true);
        clientModel.CurrentTablePosition = TablesManager.Instance.FreeTable(clientModel.CurrentTablePosition);
    }
}
