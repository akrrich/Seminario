using UnityEngine;

public enum PlayerStates
{
    Idle, Walk, Run, Jump, Cook, Admin
}

public class PlayerModel : MonoBehaviour
{
    private PlayerCamera playerCamera;
    private InventoryManager inventoryManager; // Manager del Inventario

    private Rigidbody rb;
    private Transform inventory; // GameObject hijo del player que representa el inventario
    private Transform cookingPosition;
    private Transform administratingPosition;
    private GameObject dish; // GameObject hijo del player que representa la bandeja
    private GameObject currentItem = null;
    private GameObject oven;
    private GameObject administration;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;

    private float speed;

    private bool isGrounded = true;
    private bool isCollidingOven = false;
    private bool isCollidingItem = false;
    private bool isCollidingAdministration = false;
    private bool isCoking = false;
    private bool isAdministrating = false;

    public PlayerCamera PlayerCamera { get => playerCamera; set => playerCamera = value; }
    public InventoryManager InventoryManager { get => inventoryManager; }

    public Rigidbody Rb { get => rb; }
    public Transform Inventory { get => inventory; }
    public Transform CookingPosition { get => cookingPosition; }
    public Transform AdministratingPosition { get => administratingPosition; }
    public GameObject Dish { get => dish; }
    public GameObject CurrentItem { get => currentItem; set => currentItem = value; }

    public float WalkSpeed { get => walkSpeed; }
    public float RunSpeed { get => runSpeed; } 
    public float Speed { get => speed; set => speed = value; }
    public float JumpForce { get => jumpForce; }
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
    public bool IsCollidingOven { get => isCollidingOven; set => isCollidingOven = value; }
    public bool IsCollidingItem { get => isCollidingItem; set => isCollidingItem = value; }
    public bool IsCollidingAdministration { get => isCollidingAdministration; set => isCollidingAdministration = value; }   
    public bool IsCooking { get => isCoking; set => isCoking = value; }
    public bool IsAdministrating { get => isAdministrating; set => isAdministrating = value; }


    void Awake()
    {
        GetComponents();
    }

    void FixedUpdate()
    {
        Movement();
    }


    public Vector2 GetMoveAxis()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    // Solucionar los LineOfSight
    public bool IsLookingAtOven()
    {
        return LineOfSight.LOS(playerCamera.transform, oven.transform, 5, 125, LayerMask.NameToLayer("LOS"));
    }

    public bool IsLookingAtAdministration()
    {
        return LineOfSight.LOS(playerCamera.transform, administration.transform, 5, 125, LayerMask.NameToLayer("LOS"));
    }

    public void ShowOrHideDish(bool current)
    {
        dish.SetActive(current);
    }


    private void GetComponents()
    {
        playerCamera = GetComponentInChildren<PlayerCamera>();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        rb = GetComponent<Rigidbody>();
        inventory = transform.Find("Inventory").transform;
        cookingPosition = GameObject.Find("CookingPosition").transform;
        administratingPosition = GameObject.Find("AdministratingPosition").transform;
        dish = transform.Find("Dish").gameObject;
        oven = GameObject.FindGameObjectWithTag("Oven");
        administration = GameObject.FindGameObjectWithTag("Administration");
    }

    private void Movement()
    {
        if (!isCoking && !isAdministrating)
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
