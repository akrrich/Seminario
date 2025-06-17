using UnityEngine;

public class ClientController : MonoBehaviour
{
    private ClientModel clientModel;
    private ClientView clientView;

    private ClientStateLeave<ClientStates> csLeave;
    private ClientStateEating<ClientStates> csEating;

    private FSM<ClientStates> fsm = new FSM<ClientStates>();
    private ITreeNode root;

    private bool onCollisionEnterWithTrigger = false;

    public bool OnCollisionEnterWithTrigger { get => onCollisionEnterWithTrigger; set => onCollisionEnterWithTrigger = value; }


    void Awake()
    {
        GetComponents();
    }

    void Start()
    {
        // Obligatorio inicializar la maquina de estados despues del awake, ya que depende del awake del Model
        InitializeFSM();
        InitializeTree();
    }

    void Update()
    {
        fsm.OnExecute();
        root.Execute();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Chair"))
        {
            onCollisionEnterWithTrigger = true;
        }
    }


    private void GetComponents()
    {
        clientModel = GetComponent<ClientModel>();
        clientView = GetComponent<ClientView>();
    }

    private void InitializeFSM()
    {
        ClientStateIdle<ClientStates> csIdle = new ClientStateIdle<ClientStates>(clientModel, clientView);
        ClientStateGoChair<ClientStates> csChair = new ClientStateGoChair<ClientStates>(clientModel, clientView, () => clientModel.CurrentTablePosition.ChairPosition);
        csLeave = new ClientStateLeave<ClientStates>(this, clientModel, clientView, clientModel.ClientManager.OutsidePosition);
        csEating = new ClientStateEating<ClientStates>(clientModel, clientView, csLeave);
        ClientStateWaitingFood<ClientStates> csWaitingFood = new ClientStateWaitingFood<ClientStates>(clientModel, clientView, csLeave, csEating);
        ClientStateWaitingForChair<ClientStates> csWaitingForChair = new ClientStateWaitingForChair<ClientStates>(clientModel, clientView, csLeave);

        csIdle.AddTransition(ClientStates.GoChair, csChair);
        csIdle.AddTransition(ClientStates.WaitingForChair, csWaitingForChair);

        csChair.AddTransition(ClientStates.WaitingFood, csWaitingFood);

        csWaitingFood.AddTransition(ClientStates.Leave, csLeave);
        csWaitingFood.AddTransition(ClientStates.Eating, csEating);

        csLeave.AddTransition(ClientStates.Idle, csIdle);

        csWaitingForChair.AddTransition(ClientStates.GoChair, csChair);
        csWaitingForChair.AddTransition(ClientStates.Leave, csLeave);

        csEating.AddTransition(ClientStates.Leave, csLeave);

        fsm.SetInit(csWaitingForChair);
    }

    private void InitializeTree()
    {
        ActionNode idle = new ActionNode(() => fsm.TransitionTo(ClientStates.Idle));
        ActionNode goChair = new ActionNode(() => fsm.TransitionTo(ClientStates.GoChair));
        ActionNode waitingFood = new ActionNode(() => fsm.TransitionTo(ClientStates.WaitingFood));
        ActionNode leave = new ActionNode(() => fsm.TransitionTo(ClientStates.Leave));
        ActionNode waitingForChair = new ActionNode(() => fsm.TransitionTo(ClientStates.WaitingForChair));
        ActionNode eating = new ActionNode(() => fsm.TransitionTo(ClientStates.Eating));

        // Orden: WaitingForChair, GoChair, WaitingFood, Eating, Leave, Idle

        QuestionNode qLeaveOrEat = new QuestionNode(QuestionLeaveOrEat, eating, leave);
        QuestionNode qIsWaitingForFood = new QuestionNode(QuestionIsWaitingForFood, qLeaveOrEat, waitingFood);
        QuestionNode qCanGoToChair = new QuestionNode(QuestionCanGoToChair, goChair, qIsWaitingForFood);
        QuestionNode qIsChairFreeOrNoT = new QuestionNode(QuestionIsChairFreeOrNot, qCanGoToChair, waitingForChair);
        QuestionNode qIsOutside = new QuestionNode(QuestionIsOutside, idle, qIsChairFreeOrNoT);

        root = qIsOutside;
    }

    private bool QuestionLeaveOrEat()
    {
        if (csEating.IsEating)
        {
            return true;
        }

        return false;
    }

    private bool QuestionIsWaitingForFood()
    {
        if (csLeave.CanLeave)
        {
            return true;
        }

        if (csEating.IsEating)
        {
            return true;
        }

        return false;
    }

    private bool QuestionCanGoToChair()
    {
        if (clientModel.CurrentTablePosition != null)
        {
            // si no esta colisionando con el trigger de la silla
            if (!onCollisionEnterWithTrigger)
            {
                return true;
            }

            return false;
        }

        return false;
    }

    private bool QuestionIsChairFreeOrNot()
    {
        if (csLeave.CanLeave)
        {
            return true;
        }

        if (clientModel.CurrentTablePosition != null)
        {
            return true;
        }

        return false;
    }

    private bool QuestionIsOutside()
    {                                                            // si esta dentro del rango de OutsidePosition
        if (Vector3.Distance(clientModel.ClientManager.OutsidePosition.position, transform.position) <= 2f)
        {
            return true;
        }

        return false;
    }
}
