using System.Collections;
using UnityEngine;

public class ClientStateWaitingFood<T> : State<T>
{
    private ClientModel clientModel;
    private ClientView clientView;

    private ClientStateLeave<T> clientStateLeave;

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

        waitingFoodTime += Time.deltaTime;

        if (waitingFoodTime >= clientModel.MaxTimeWaitingFood)
        {
            clientStateLeave.SetInLeaveManualy = true;
            waitingFoodTime = 0f;
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();

        clientView.Anim.transform.position += Vector3.down * 0.38f;
        waitingFoodTime = 0f;
        clientModel.ClientManager.SetParentToHisPoolGameObject(clientModel.ClientType, clientModel);
    }


    private IEnumerator DuringSitAnimationAfterExitTime()
    {
        yield return new WaitForSeconds(4.12f); // Tiempo que tarda en sentarse por completo
        
        clientView.ExecuteAnimParameterName("DuringSit");
        clientView.Anim.transform.position += Vector3.up * 0.38f;
        clientView.SetSpriteType("SpriteWaitingFood");
    }
}
