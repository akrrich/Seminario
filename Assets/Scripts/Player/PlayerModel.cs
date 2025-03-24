using UnityEngine;

public enum PlayerStates
{
    Idle,
    Walk,
    Jump,
    Cook
}

public class PlayerModel : MonoBehaviour
{
    [SerializeField] public Transform cookPosition;

    private PlayerCamera playerCamera;
    private Rigidbody rb;

    [SerializeField] private float speed = 250f;
    [SerializeField] private float jumpForce = 5f;

    private bool isGrounded = true;
    private bool isCollidingOven = false;
    private bool isCoking = false;

    public PlayerCamera PlayerCamera { get => playerCamera; set => playerCamera = value; }
    public Rigidbody Rb { get => rb; set => rb = value; }

    public float JumpForce { get => jumpForce; }
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
    public bool IsCollidingOven { get => isCollidingOven; set => isCollidingOven = value; }
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


    private void GetComponents()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<PlayerCamera>();
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
