using System.Collections;
using UnityEngine;

public class ClientStateWaitingFood<T> : State<T>
{
    private ClientModel clientModel;
    private ClientView clientView;

    private ClientStateLeave<T> clientStateLeave;

    private bool canExecuteTimers = false;

    private float waitingToBeAttendedTime = 0f;
    private float waitingFoodTime = 0f;


    public ClientStateWaitingFood(ClientModel clientModel, ClientView clientView, ClientStateLeave<T> clientStateLeave)
    {
        this.clientModel = clientModel;
        this.clientView = clientView;
        this.clientStateLeave = clientStateLeave;
    }


    public override void Enter()
    {
        base.Enter();
        Debug.Log("WaitingFood");

        clientModel.StopVelocity();
        clientModel.LookAt(clientModel.CurrentTablePosition.transform.position, clientView.Anim.transform);
        clientView.ExecuteAnimParameterName("Sit");
        clientView.StartCoroutine(DuringSitAnimationAfterExitTime());
        clientModel.transform.SetParent(clientModel.CurrentTablePosition.ChairPosition);
    }

    public override void Execute()
    {
        base.Execute();

        if (canExecuteTimers)
        {
            // Ejecuta el tiempo a ser atendido
            if (clientView.ReturnSpriteWaitingFoodIsActive())
            {
                waitingToBeAttendedTime += Time.deltaTime;

                if (waitingToBeAttendedTime >= clientModel.MaxTimeWaitingToBeAttended)
                {
                    clientStateLeave.SetInLeaveManualy = true;
                    waitingToBeAttendedTime = 0f;
                    canExecuteTimers = false;
                    return;
                }
            }

            // Ejecuta el tiempo a recibir el pedido
            else
            {
                waitingFoodTime += Time.deltaTime;

                if (waitingFoodTime >= clientModel.MaxTimeWaitingFood)
                {
                    clientStateLeave.SetInLeaveManualy = true;
                    waitingFoodTime = 0f;
                    canExecuteTimers = false;
                    return;
                }
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        clientView.Anim.transform.position += Vector3.down * 0.38f;
        waitingToBeAttendedTime = 0f;
        waitingFoodTime = 0f;
        clientModel.ClientManager.SetParentToHisPoolGameObject(clientModel.ClientType, clientModel);
        canExecuteTimers = false;
    }


    private IEnumerator DuringSitAnimationAfterExitTime()
    {
        yield return new WaitForSeconds(4.12f); // Tiempo que tarda en sentarse por completo
        
        clientView.ExecuteAnimParameterName("DuringSit");
        clientView.Anim.transform.position += Vector3.up * 0.38f;
        clientView.SetSpriteTypeName("SpriteWaitingFood");
        canExecuteTimers = true;
    }
}
