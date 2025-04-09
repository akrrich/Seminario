using UnityEngine;

public class ClientController : MonoBehaviour
{
    private ClientModel clientModel;
    private ClientView clientView;

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(clientModel.CurrentTablePosition.ChairPosition.position, 2);
    }


    private void GetComponents()
    {
        clientModel = GetComponent<ClientModel>();
        clientView = GetComponent<ClientView>();
    }

    private void InitializeFSM()
    {
        ClientStateIdle<ClientStates> csIdle = new ClientStateIdle<ClientStates>(clientModel);
        ClientStateGoChair<ClientStates> csChair = new ClientStateGoChair<ClientStates>(clientModel, clientModel.CurrentTablePosition.ChairPosition);
        ClientStateWaiting<ClientStates> csWaitingFood = new ClientStateWaiting<ClientStates>(clientModel);
        ClientStateLeave<ClientStates> csLeave = new ClientStateLeave<ClientStates>(clientModel, clientModel.ClientManager.OutsidePosition);

        csIdle.AddTransition(ClientStates.GoChair, csChair);

        csChair.AddTransition(ClientStates.WaitingFood, csWaitingFood);

        csWaitingFood.AddTransition(ClientStates.Leave, csLeave);

        csLeave.AddTransition(ClientStates.idle, csIdle);

        fsm.SetInit(csChair); // Intercambiar estados entre idle y chair segun se necesite
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
        QuestionNode qIsOutside = new QuestionNode(QuestionIsOutside, idle, qCanLeave);

        root = qIsOutside;
    }

    private bool QuestionIsWaitingForFood()
    {
        if (clientModel.CurrentTablePosition.DishPosition.transform.childCount > 0)
        {
            if (clientModel.CurrentTablePosition.DishPosition.transform.GetChild(0).name == clientView.FoodName + "(Clone)")
            {
                arrivalTime += Time.deltaTime;

                if (arrivalTime >= 5f)
                {
                    arrivalTime = 0f;

                    clientModel.CurrentTablePosition.CurrentFood.ReturnObjetToPool();
                    //clientModel.CurrentTablePosition.CurrentFood = null;

                    clientModel.ClientManager.FreeTable(clientModel.CurrentTablePosition);
                    return true;
                }
            }
        }

        return false;
    }

    private bool QuestionCanGoToChair()
    {                                                    // si esta fuera del rango de la silla
        if (Vector3.Distance(clientModel.CurrentTablePosition.ChairPosition.position, transform.position) > 2f)
        {
            return true;
        }

        return false;
    }

    private bool QuestionLeave()
    {                                                    // si esta dentro del rango de la silla
        if (Vector3.Distance(clientModel.CurrentTablePosition.ChairPosition.position, transform.position) <= 1f)
        {
            return true;
        }

        return false;
    }

    private bool QuestionIsOutside()
    {                                                    // si esta afuera de la taberna
        if (Vector3.Distance(clientModel.ClientManager.OutsidePosition.position, transform.position) <= 2f)
        {
            return true;
        }

        return false;
    }
}
