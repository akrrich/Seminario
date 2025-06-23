using UnityEngine;
using UnityEngine.AI;

public enum ClientStates
{
    Idle, GoChair, WaitingFood, WaitingForChair, Leave, Eating
}

public enum ClientType
{
    Ogre, Orc, Goblin 
}

public class ClientModel : MonoBehaviour
{
    /// <summary>
    /// Analizar el tema del Capusle collider, ya que se desincroniza del Animation gameObject
    /// </summary>
    /// 
    [SerializeField] private ClientData clientData;

    private ClientManager clientManager;

    private Rigidbody rb;
    private NavMeshAgent navMeshAgent;
    private CapsuleCollider capsuleCollider;
    private Table currentTablePosition;

    private Vector3 currentDirection;

    [SerializeField] private ClientType clientType;

    private bool isInstantiateFirstTime = true;

    public ClientData ClientData { get => clientData; }

    public ClientManager ClientManager { get => clientManager; }

    public NavMeshAgent NavMeshAgent { get => navMeshAgent; }
    public Table CurrentTablePosition { get => currentTablePosition; set => currentTablePosition = value; }

    public ClientType ClientType { get => clientType; }


    void Awake()
    {
        GetComponents();
        InitializeValuesFromNavMeshAgent();
    }

    void OnEnable()
    {
        InitializeClientForPool();
    }

    void FixedUpdate()
    {
        Movement();
    }


    public void MoveToTarget(Vector3 target)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(target);
    }

    public void LookAt(Vector3 target, Transform anim)
    {
        Vector3 newDirection = (target - transform.position).normalized;
        Vector3 lookDirection = new Vector3(newDirection.x, 0, newDirection.z);

        if (lookDirection != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = rotation;
            anim.transform.rotation = rotation;
        }
    }

    public void StopVelocity()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
    }

    /// Metodo para futuro setea del npc correctamente
    public void SetRbAndCollider(bool rb, bool collider)
    {
        this.rb.isKinematic = rb;
        capsuleCollider.enabled = collider;
    }

    public void ReturnFoodFromTableToPool(bool dishesMatch)
    {
        foreach (Food food in currentTablePosition.CurrentFoods)
        {
            food.ReturnObjetToPool();
        }

        currentTablePosition.CurrentFoods.Clear();

        if (dishesMatch)
        {
            MoneyManager.Instance.AddMoney(500);
        }

        else
        {
            MoneyManager.Instance.SubMoney(250);
        }
    }


    private void GetComponents()
    {
        clientManager = FindFirstObjectByType<ClientManager>();

        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        capsuleCollider =  GetComponent<CapsuleCollider>();
    }

    private void InitializeValuesFromNavMeshAgent()
    {
        navMeshAgent.speed = clientData.Speed;
    }

    public void InitializeSpawnPosition()
    {
        transform.position = clientManager.SpawnPosition.position;
    }

    public void InitializeTablePosition()
    {
        currentTablePosition = TablesManager.Instance.GetRandomAvailableTableForClient();
    }

    private void InitializeClientForPool()
    {
        if (!isInstantiateFirstTime)
        {
            InitializeSpawnPosition();
            InitializeTablePosition();
            return;
        }

        isInstantiateFirstTime = false;
    }

    private void Movement()
    {
        if (!rb.isKinematic)
        {
            rb.velocity = currentDirection * clientData.Speed * Time.fixedDeltaTime;
        }
    }
}
