using UnityEngine;

public class ClientController : MonoBehaviour
{
    private ClientModel clientModel;

    private FSM<ClientStates> fsm = new FSM<ClientStates>();
    private ITreeNode root;

    private float arrivalTime = 0f; // Provisorio


    void Awake()
    {
        GetComponents();
        InitializeFSM();
        InitializeTree();
    }

    void Update()
    {
        fsm.OnExecute();
        root.Execute();
    }


    private void GetComponents()
    {
        clientModel = GetComponent<ClientModel>();
    }

    private void InitializeFSM()
    {
        ClientStateIdle<ClientStates> csIdle = new ClientStateIdle<ClientStates>(clientModel);
        ClientStateGoChair<ClientStates> csChair = new ClientStateGoChair<ClientStates>(clientModel, clientModel.CurrentTablePosition);
        ClientStateWaiting<ClientStates> csWaitingFood = new ClientStateWaiting<ClientStates>(clientModel);
        ClientStateLeave<ClientStates> csLeave = new ClientStateLeave<ClientStates>(clientModel, clientModel.ClientManager.OutsidePosition);

        csIdle.AddTransition(ClientStates.GoChair, csChair);

        csChair.AddTransition(ClientStates.WaitingFood, csWaitingFood);

        csWaitingFood.AddTransition(ClientStates.Leave, csLeave);

        csLeave.AddTransition(ClientStates.idle, csIdle);

        fsm.SetInit(csIdle);
    }

    private void InitializeTree()
    {
        ActionNode idle = new ActionNode(() => fsm.TransitionTo(ClientStates.idle));
        ActionNode goChair = new ActionNode(() => fsm.TransitionTo(ClientStates.GoChair));
        ActionNode waitingFood = new ActionNode(() => fsm.TransitionTo(ClientStates.WaitingFood));
        ActionNode leave = new ActionNode(() => fsm.TransitionTo(ClientStates.Leave));

        QuestionNode qIsWaitingForFood = new QuestionNode(QuestionIsWaitingForFood, leave, waitingFood);
        QuestionNode qCanGoToChair = new QuestionNode(QuestionCanGoToChair, goChair, qIsWaitingForFood);
        QuestionNode qCanLeave = new QuestionNode(QuestionLeave, leave, qCanGoToChair);

        root = qCanLeave;
    }

    private bool QuestionIsWaitingForFood()
    {
        arrivalTime += Time.deltaTime;

        if (arrivalTime >= 5f)
        {
            arrivalTime = 0f;
            clientModel.ClientManager.FreeTable(clientModel.CurrentTablePosition);
            return true;
        }

        return false;
    }

    private bool QuestionCanGoToChair()
    {                                                                                         // si esta fuera del rango de la silla
        if (Vector3.Distance(clientModel.CurrentTablePosition.position, transform.position) >= 15f)
        {
            return true;
        }

        return false;
    }

    private bool QuestionLeave()
    {                                                                                     // si esta dentro del rango de la silla
        if (Vector3.Distance(clientModel.CurrentTablePosition.position, transform.position) <= 2f)
        {
            return true;
        }

        return false;
    }
}
