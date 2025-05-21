using System.Collections.Generic;
using UnityEngine;

public class ClientController : MonoBehaviour
{
    private ClientModel clientModel;
    private ClientView clientView;

    private ClientStateLeave<ClientStates> csLeave;

    private FSM<ClientStates> fsm = new FSM<ClientStates>();
    private ITreeNode root;

    private float arrivalTime = 0f; // Provisorio


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


    private void GetComponents()
    {
        clientModel = GetComponent<ClientModel>();
        clientView = GetComponent<ClientView>();
    }

    private void InitializeFSM()
    {
        ClientStateIdle<ClientStates> csIdle = new ClientStateIdle<ClientStates>(clientModel);
        ClientStateGoChair<ClientStates> csChair = new ClientStateGoChair<ClientStates>(clientModel, clientView, () => clientModel.CurrentTablePosition.ChairPosition);
        csLeave = new ClientStateLeave<ClientStates>(clientModel, clientView, clientModel.ClientManager.OutsidePosition);
        ClientStateWaitingFood<ClientStates> csWaitingFood = new ClientStateWaitingFood<ClientStates>(clientModel, clientView, csLeave);
        ClientStateWaitingForChair<ClientStates> csWaitingForChair = new ClientStateWaitingForChair<ClientStates>(clientModel, clientView, csLeave);

        csIdle.AddTransition(ClientStates.GoChair, csChair);
        csIdle.AddTransition(ClientStates.WaitingForChair, csWaitingForChair);

        csChair.AddTransition(ClientStates.WaitingFood, csWaitingFood);

        csWaitingFood.AddTransition(ClientStates.Leave, csLeave);

        csLeave.AddTransition(ClientStates.Idle, csIdle);

        csWaitingForChair.AddTransition(ClientStates.GoChair, csChair);
        csWaitingForChair.AddTransition(ClientStates.Leave, csLeave);

        fsm.SetInit(csWaitingForChair);
    }

    private void InitializeTree()
    {
        ActionNode idle = new ActionNode(() => fsm.TransitionTo(ClientStates.Idle));
        ActionNode goChair = new ActionNode(() => fsm.TransitionTo(ClientStates.GoChair));
        ActionNode waitingFood = new ActionNode(() => fsm.TransitionTo(ClientStates.WaitingFood));
        ActionNode leave = new ActionNode(() => fsm.TransitionTo(ClientStates.Leave));
        ActionNode waitingForChair = new ActionNode(() => fsm.TransitionTo(ClientStates.WaitingForChair));

        // Orden: WaitingForChair, GoChair, WaitingFood, Leave, Idle

        QuestionNode qIsWaitingForFood = new QuestionNode(QuestionIsWaitingForFood, leave, waitingFood);
        QuestionNode qCanGoToChair = new QuestionNode(QuestionCanGoToChair, goChair, qIsWaitingForFood);
        QuestionNode qIsChairFreeOrNoT = new QuestionNode(QuestionIsChairFreeOrNot, qCanGoToChair, waitingForChair);
        //QuestionNode qCanLeave = new QuestionNode(QuestionLeave, leave, qIsChairFreeOrNoT);
        QuestionNode qIsOutside = new QuestionNode(QuestionIsOutside, idle, qIsChairFreeOrNoT);

        root = qIsOutside;
    }

    private bool QuestionIsWaitingForFood()
    {
        if (csLeave.SetInLeaveManualy)
        {
            return true;
        }

        List<string> expectedDishNames = new List<string>();
        List<string> servedDishNames = new List<string>();

        foreach (string food in clientView.OrderFoodNames)
        {
            expectedDishNames.Add(food + "(Clone)");
        }

        foreach (Transform dishSpot in clientModel.CurrentTablePosition.DishPositions)
        {
            if (dishSpot.childCount > 0)
            {
                servedDishNames.Add(dishSpot.GetChild(0).name);
            }
        }

        if (servedDishNames.Count == 0)
        {
            arrivalTime = 0f;
            return false;
        }

        expectedDishNames.Sort();
        servedDishNames.Sort();

        bool dishesMatch = expectedDishNames.Count == servedDishNames.Count;

        if (dishesMatch)
        {
            for (int i = 0; i < expectedDishNames.Count; i++)
            {
                if (expectedDishNames[i] != servedDishNames[i])
                {
                    dishesMatch = false;
                    break;
                }
            }
        }

        if (!dishesMatch)
        {
            return clientModel.ReturnFoodFromTableToPool(ref arrivalTime, false);
        }

        return clientModel.ReturnFoodFromTableToPool(ref arrivalTime, true);
    }

    private bool QuestionCanGoToChair()
    {
        if (clientModel.CurrentTablePosition != null)
        {
            // si esta fuera del rango de la silla (lejos de la silla)
            if (Vector3.Distance(clientModel.CurrentTablePosition.ChairPosition.position, transform.position) > 2f)
            {
                return true;
            }

            return false;
        }

        return false;
    }

    /*private bool QuestionLeave()
    {
        if (clientModel.CurrentTablePosition != null) 
        {
                                                                // si esta dentro del rango de la silla
            if (Vector3.Distance(clientModel.CurrentTablePosition.ChairPosition.position, transform.position) <= 1f)
            {
                return true;
            }

            return false;
        }

        return false;
    }*/

    private bool QuestionIsOutside()
    {                                                            // si esta dentro del rango de OutsidePosition
        if (Vector3.Distance(clientModel.ClientManager.OutsidePosition.position, transform.position) <= 2f)
        {
            return true;
        }

        return false;
    }

    private bool QuestionIsChairFreeOrNot()
    {
        if (csLeave.SetInLeaveManualy)
        {
            return true;
        }

        if (clientModel.CurrentTablePosition != null)
        {
            return true;
        }

        return false;
    }
}
