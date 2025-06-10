using UnityEngine;

public class PlayerDungeonModel : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float speed;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float airMultiplier = 5f;
    [SerializeField] private float groundDrag;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float dashCooldown = 1.5f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool showGroundGizmo = true;

    private float currentHP;
    private float lastDashTime = -Mathf.Infinity;
    
    private bool isDead = false;
    private bool invulnerable = false;
    private bool canDash = true;
    
    private Vector3 _moveDirection;
    private Transform orientation;
    private Rigidbody rb;
    private DashHandler dash;
    private CombatHandler combat;

    #region PROPERTIES
    // === Properties ===
    public float Speed { get => speed; set => speed = value; }
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
    public float JumpForce => jumpForce;
    public float DashCooldown => dashCooldown;
    public bool CanDash => Time.time >= lastDashTime + dashCooldown;

    public Rigidbody Rb => rb;
    public float CurrentHP => currentHP;
    public bool IsDead => isDead;
    public bool IsGrounded => Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    public bool IsFalling => rb.velocity.y < -0.1f && !IsGrounded;
    public Vector3 MoveDirection { get=> _moveDirection; set=>_moveDirection=value; }
    public bool IsInvulnerable { get; private set; }
    public bool CanMove { get; set; } = true;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        dash = GetComponent<DashHandler>();
        combat = GetComponent<CombatHandler>();
        orientation = GetComponent<Transform>();
        rb.freezeRotation = true;

        currentHP = maxHP;
       
    }
    private void FixedUpdate()
    {
        MovePlayer();
        
    }
    private void Update()
    {
        Inputs();
        SpeedControl();

        if (IsGrounded) rb.drag = groundDrag;
        else rb.drag = 0;
    }
    public void TakeDamage(int amount)
    {
        if (invulnerable) return;

        currentHP -= amount;
        if (currentHP <= 0)
        {
            currentHP = 0;
            isDead = true;
        }
    }
    public void SetInvulnerable(bool value)
    {
        invulnerable = value;
    }
    public void RegisterDash() => lastDashTime = Time.time;
   
    private void Inputs()
    {
        if (PlayerInputs.Instance.Jump())
        {
            Jump();
        }
        if (PlayerInputs.Instance.Dash())
        {
            ExecuteDash();
        }
        
    }
    private void MovePlayer()
    {
        _moveDirection = orientation.forward * PlayerInputs.Instance.GetMoveAxis().x + orientation.right * PlayerInputs.Instance.GetMoveAxis().y;

        //on ground
        if (IsGrounded)
            rb.AddForce(_moveDirection * walkSpeed * 10f, ForceMode.Force);

        //in air
        else if (!IsGrounded)
            rb.AddForce(_moveDirection * walkSpeed * 10f * airMultiplier, ForceMode.Force);
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > walkSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * walkSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    private void Jump()
    {
        if (IsGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }
    private void ExecuteDash()
    {
        dash.ExecuteDash();
    }
    
}
