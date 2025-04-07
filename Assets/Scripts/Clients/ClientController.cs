using System.Collections;
using UnityEngine;

public class ClientController : MonoBehaviour
{
    private ClientModel clientModel;

    private FSM<ClientStates> fsm = new FSM<ClientStates>();
    private ITreeNode root;


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

        print(root);
    }


    private void GetComponents()
    {
        clientModel = GetComponent<ClientModel>();
    }

    private void InitializeFSM()
    {
        ClientStateIdle<ClientStates> csIdle = new ClientStateIdle<ClientStates>(clientModel);
        ClientStateChase<ClientStates> csChair = new ClientStateChase<ClientStates>(clientModel, clientModel.newTransform);
        ClientStateChase<ClientStates> csLeave = new ClientStateChase<ClientStates>(clientModel, clientModel.startTransform);

        csIdle.AddTransition(ClientStates.GoChair, csChair);
        csIdle.AddTransition(ClientStates.Leave, csLeave);

        csChair.AddTransition(ClientStates.idle, csIdle);
        csChair.AddTransition(ClientStates.Leave, csLeave);

        csLeave.AddTransition(ClientStates.idle, csIdle);

        fsm.SetInit(csIdle);
    }

    private void InitializeTree()
    {
        ActionNode idle = new ActionNode(() => fsm.TransitionTo(ClientStates.idle));
        ActionNode goChair = new ActionNode(() => fsm.TransitionTo(ClientStates.GoChair));
        ActionNode leave = new ActionNode(() => fsm.TransitionTo(ClientStates.Leave));

        QuestionNode qCanGoToChair = new QuestionNode(QuestionCanGoToChair, goChair, idle);
        QuestionNode qCanLeave = new QuestionNode(QuestionLeave, leave, qCanGoToChair);

        root = qCanLeave;
    }

    private bool QuestionCanGoToChair()
    {
        if (clientModel.newTransform != null)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    private bool Question()
    {
        if (QuestionLeave())
        {
            return false;
        }

        else
        {
            StartCoroutine(waitSeconds());
        }

        return false;
    }

    private bool QuestionLeave()
    {
        return Vector3.Distance(clientModel.newTransform.position, transform.position) <= 1f;
    }

    private IEnumerator waitSeconds()
    {
        yield return new WaitForSeconds(3);
    }
}
