using UnityEngine;

public enum ClientStates
{
    idle, GoChair, WaitingFood, Leave
}

public class ClientModel : MonoBehaviour
{
    private ClientManager clientManager;

    private Rigidbody rb;
    private Transform currentTablePosition;

    private Vector3 currentDirection;

    [SerializeField] public float speed;

    public ClientManager ClientManager { get => clientManager; }

    public Transform CurrentTablePosition { get => currentTablePosition; }


    void Awake()
    {
        GetComponents();
        InitializeTablePosition();
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


    private void GetComponents()
    {
        clientManager = FindFirstObjectByType<ClientManager>();

        rb = GetComponent<Rigidbody>();
    }

    private void InitializeTablePosition()
    {
        currentTablePosition = clientManager.GetRandomAvailableTable();
    }

    private void Movement()
    {
        rb.velocity = currentDirection * speed * Time.fixedDeltaTime;
    }
}
