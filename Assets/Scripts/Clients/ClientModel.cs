using UnityEngine;

public enum ClientStates
{
    Idle, GoChair, WaitingFood, WaitingForChair, Leave
}

public enum ClientType
{
    Ogro, Orc, Goblin 
}

public class ClientModel : MonoBehaviour
{
    private ClientManager clientManager;

    private Rigidbody rb;
    private Table currentTablePosition;

    private Vector3 currentDirection;

    [SerializeField] private ClientType clientType;

    [SerializeField] private float speed;
    [SerializeField] private float maxTimeWaitingForChair;
    [SerializeField] private float maxTimeWaitingToBeAttended;
    [SerializeField] private float maxTimeWaitingFood;

    private bool isInstantiateFirstTime = true;

    public ClientManager ClientManager { get => clientManager; }

    public Table CurrentTablePosition { get => currentTablePosition; set => currentTablePosition = value; }

    public ClientType ClientType { get => clientType; }

    public float MaxTimeWaitingForChair { get => maxTimeWaitingForChair; }
    public float MaxTimeWaitingToBeAttended { get => maxTimeWaitingToBeAttended; }
    public float MaxTimeWaitingFood { get => maxTimeWaitingFood; }


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

    public void LookAt(Vector3 target, Transform animTransform)
    {
        Vector3 newDirection = (target - transform.position).normalized;
        Vector3 lookDirection = new Vector3(newDirection.x, 0, newDirection.z);

        if (lookDirection != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = rotation;
            animTransform.transform.rotation = rotation;
        }
    }

    public void StopVelocity()
    {
        currentDirection = Vector3.zero;
    }

    public bool ReturnFoodFromTableToPool(ref float arrivalTime, bool dishesMatch)
    {
        arrivalTime += Time.deltaTime;

        if (arrivalTime >= 5f)
        {
            arrivalTime = 0f;

            foreach (Food food in currentTablePosition.CurrentFoods)
            {
                food.ReturnObjetToPool();
            }

            currentTablePosition.CurrentFoods.Clear();
            clientManager.FreeTable(currentTablePosition);

            if (dishesMatch)
            {
                MoneyManager.Instance.AddMoney(500);
            }

            else
            {
                MoneyManager.Instance.SubMoney(250);
            }


            return true;
        }

        return false;
    }


    private void GetComponents()
    {
        clientManager = FindFirstObjectByType<ClientManager>();

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
        rb.velocity = currentDirection * speed * Time.fixedDeltaTime;
    }
}
