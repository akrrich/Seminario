using UnityEngine;

public class ClientStateWaitingForChair<T> : State<T>
{
    private ClientModel clientModel;
    private ClientView clientView;

    private ClientStateLeave<T> clientStateLeave;

    private float waitingForChairTime = 0f;


    public ClientStateWaitingForChair(ClientModel clientModel, ClientView clientView, ClientStateLeave<T> clientStateLeave)
    {
        this.clientModel = clientModel;
        this.clientView = clientView;
        this.clientStateLeave = clientStateLeave;
    }


    public override void Enter()
    {
        base.Enter();
        Debug.Log("WaitingForChair");

        clientModel.StopVelocity();
        clientModel.LookAt(clientModel.transform.position, clientView.Anim.transform); // Aca hay que modificar y poner un transform de la taberna para que mire a la taberna cuando se pone en la cola
        clientView.ExecuteAnimParameterName("WaitingForChair");
    }

    public override void Execute()
    {
        base.Execute();

        if (clientModel.CurrentTable == null)
        {
            clientModel.CurrentTable = TablesManager.Instance.GetRandomAvailableTableForClient();
        }

        waitingForChairTime += Time.deltaTime;

        if (waitingForChairTime >= clientModel.ClientData.MaxTimeWaitingForChair)
        {
            clientStateLeave.CanLeave = true;
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();

        waitingForChairTime = 0f;
    }
}
