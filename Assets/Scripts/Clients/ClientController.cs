using System.Collections.Generic;
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
        ClientStateGoChair<ClientStates> csChair = new ClientStateGoChair<ClientStates>(clientModel, () => clientModel.CurrentTablePosition.ChairPosition);
        ClientStateWaiting<ClientStates> csWaitingFood = new ClientStateWaiting<ClientStates>(clientModel);
        ClientStateLeave<ClientStates> csLeave = new ClientStateLeave<ClientStates>(clientModel, clientModel.ClientManager.OutsidePosition);

        csIdle.AddTransition(ClientStates.GoChair, csChair);

        csChair.AddTransition(ClientStates.WaitingFood, csWaitingFood);

        csWaitingFood.AddTransition(ClientStates.Leave, csLeave);

        csLeave.AddTransition(ClientStates.idle, csIdle);

        fsm.SetInit(csChair);
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
    {                                                    // si esta afuera del rango de OutsidePosition
        if (Vector3.Distance(clientModel.ClientManager.OutsidePosition.position, transform.position) <= 2f)
        {
            return true;
        }

        return false;
    }
}
