using System;
using UnityEngine;

public enum ClientStates
{
    idle, GoChair, WaitingFood, Leave
}

public class ClientModel : MonoBehaviour
{
    private ClientManager clientManager;

    private Rigidbody rb;
    private Table currentTablePosition;

    private static Action onWaitingFood;

    private Vector3 currentDirection;

    [SerializeField] private float speed;

    private bool isInstantiateFirstTime = true;

    public ClientManager ClientManager { get => clientManager; }

    public Table CurrentTablePosition { get => currentTablePosition; }

    public static Action OnWaitingFood { get => onWaitingFood; set => onWaitingFood = value; }


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


    public void MoveToTarget(Transform target)
    {
        Vector3 newDirection = (target.position - transform.position).normalized;
        currentDirection = newDirection;
        Vector3 lookDirection = new Vector3(newDirection.x, 0, newDirection.z);

        if (lookDirection != Vector3.zero)
        { 
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = rotation;
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

    private void InitializeTablePosition()
    {
        currentTablePosition = clientManager.GetRandomAvailableTable();
    }

    private void InitializeClientForPool()
    {
        if (!isInstantiateFirstTime)
        {
            transform.position = clientManager.SpawnPosition.position;
            InitializeTablePosition();
            ClientView.OnFoodChangeUI?.Invoke();
        }

        isInstantiateFirstTime = false;
    }

    private void Movement()
    {
        rb.velocity = currentDirection * speed * Time.fixedDeltaTime;
    }
}
