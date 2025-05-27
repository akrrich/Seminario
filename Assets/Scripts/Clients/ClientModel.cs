using UnityEngine;

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
    [SerializeField] private ClientData clientData;

    private ClientManager clientManager;

    private ObstacleAvoidance obstacleAvoidance;
    private Rigidbody rb;
    private Table currentTablePosition;

    private Vector3 currentDirection;

    [SerializeField] private ClientType clientType;

    private bool isInstantiateFirstTime = true;

    public ClientData ClientData { get => clientData; }

    public ClientManager ClientManager { get => clientManager; }

    public ObstacleAvoidance ObstacleAvoidance { get => obstacleAvoidance; }
    public Table CurrentTablePosition { get => currentTablePosition; set => currentTablePosition = value; }

    public ClientType ClientType { get => clientType; }


    void Awake()
    {
        GetComponents();
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
        Vector3 newDirection = (target - transform.position).normalized;
        currentDirection = newDirection;
    }

    // Para ObstacleAvoidance
    /*public void MoveToTarget(Vector3 target)
    {
        currentDirection = target;
    }*/

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
        currentDirection = Vector3.zero;
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

        obstacleAvoidance = GetComponent<ObstacleAvoidance>();
        rb = GetComponent<Rigidbody>();
    }

    public void InitializeSpawnPosition()
    {
        transform.position = clientManager.SpawnPosition.position;
    }

    public void InitializeTablePosition()
    {
        currentTablePosition = clientManager.GetRandomAvailableTable();
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
        rb.velocity = currentDirection * clientData.Speed * Time.fixedDeltaTime;
    }
}
