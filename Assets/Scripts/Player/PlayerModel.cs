using UnityEngine;

public enum PlayerStates
{
    Idle, Walk, Jump, Cook, Grab
}

public class PlayerModel : MonoBehaviour
{
    private PlayerCamera playerCamera;
    private InventoryManager inventoryManager; // Manager del Inventario

    private Rigidbody rb;
    private Transform inventory; // GameObject hijo del player que representa el inventario
    private Transform cookingPosition;
    private GameObject currentItem = null;

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    private bool isGrounded = true;
    private bool isCollidingOven = false;
    private bool isCollidingItem = false;
    private bool isCoking = false;

    public PlayerCamera PlayerCamera { get => playerCamera; set => playerCamera = value; }
    public InventoryManager InventoryManager { get => inventoryManager; }

    public Rigidbody Rb { get => rb; }
    public Transform Inventory { get => inventory; }
    public Transform CookingPosition { get => cookingPosition; }
    public GameObject CurrentItem { get => currentItem; set => currentItem = value; }

    public float JumpForce { get => jumpForce; }
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
    public bool IsCollidingOven { get => isCollidingOven; set => isCollidingOven = value; }
    public bool IsCollidingItem { get => isCollidingItem; set => isCollidingItem = value; }
    public bool IsCooking { get => isCoking; set => isCoking = value; }


    void Awake()
    {
        GetComponents();
    }

    void FixedUpdate()
    {
        Movement();
    }


    public static Vector2 GetMoveAxis()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    // Provisorio
    public bool IsLookingAtOven()
    {
        // Solucionar la parte comentada

        /*GameObject oven = GameObject.FindGameObjectWithTag("Oven");

        Vector3 directionToOven = oven.transform.position - transform.position;
        float angle = Vector3.Angle(playerCamera.transform.forward, directionToOven);

        return LineOfSight.LOS(playerCamera.transform, oven.transform, 50, angle, LayerMask.NameToLayer("Oven"));*/

        GameObject oven = GameObject.FindGameObjectWithTag("Oven");

        Vector3 directionToOven = oven.transform.position - transform.position;
        float angle = Vector3.Angle(playerCamera.transform.forward, directionToOven);

        if (angle <= 90f)
        {
            return true;
        }

        return false;
    }


    private void GetComponents()
    {
        playerCamera = GetComponentInChildren<PlayerCamera>();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        rb = GetComponent<Rigidbody>();
        inventory = transform.Find("Inventory").transform;
        cookingPosition = GameObject.Find("CookingPosition").transform;
    }

    private void Movement()
    {
        if (!isCoking)
        {
            Vector3 cameraForward = playerCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            Vector3 right = playerCamera.transform.right;
            Vector3 movement = (cameraForward * GetMoveAxis().y + right * GetMoveAxis().x).normalized * speed * Time.deltaTime;

            rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
        }
    }
}
