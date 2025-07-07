using System;
using System.Collections;
using UnityEngine;

public class PlayerDungeonModel : MonoBehaviour, IDamageable
{
    #region Inspector Fields
    [Header("Stats")]
    [Header("HP")]
    [SerializeField] private float maxHP = 100f;

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float airMultiplier = 5f;
    [SerializeField] private float groundDrag = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;

    [Header("Dash")]
    [SerializeField] private float dashCooldown = 3f;   

    [Header("Combat")]
    [SerializeField] private int currentWeaponDamage = 5;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool showGroundGizmo = true;
    #endregion

    #region Private Fields
    private float currentHP;
    private float lastDashTime = -Mathf.Infinity;

    private bool isDead = false;
    private bool invulnerable = false;

    private Vector3 moveDirection;
    private Transform orientation;
    private Rigidbody rb;
    private DashHandler dashHandler;
    private CombatHandler combatHandler;
    #endregion

    #region Properties
    // -------- Estado general --------
    public float CurrentHP => currentHP;
    public bool IsDead => isDead;
    public bool IsInvulnerable => invulnerable;
    public bool CanMove { get; set; } = true;

    // -------- Movimiento --------
    public float Speed { get => speed; set => speed = value; }
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
    public float JumpForce => jumpForce;
    public Rigidbody Rb => rb;
    public Vector3 MoveDirection { get => moveDirection; set => moveDirection = value; }

    public bool IsGrounded => Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    public bool IsFalling => rb.velocity.y < -0.1f && !IsGrounded;

    // -------- Dash --------
    public bool DashEnabled { get; set; } = true;
    public bool CanDash => DashEnabled && Time.time >= lastDashTime + dashCooldown;

    // -------- Combate --------
    public int CurrentWeaponDamage { get => currentWeaponDamage; set => currentWeaponDamage = value; }
    public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }

    //------Provisorio, para el alpha------
    public event Action<float, float> OnHealthChanged;
    public event Action OnPlayerDied;
    public static Action<PlayerDungeonModel> onPlayerInitialized;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        dashHandler = GetComponent<DashHandler>();
        combatHandler = GetComponent<CombatHandler>();
        orientation = transform.Find("Orientation");

        rb.freezeRotation = true;
        currentHP = maxHP;
        OnHealthChanged?.Invoke(currentHP, maxHP);
        StartCoroutine(InvokeEventInitializationPlayer());
    }

    private void FixedUpdate()
    {
        if (PlayerInputs.Instance != null)
        {
            MovePlayer();
        }
    }

    private void Update()
    {
        HandleInputs();
        SpeedControl();
        rb.drag = IsGrounded ? groundDrag : 0f;
    }
    #endregion

    #region Public Methods
    public void TakeDamage(int amount)
    {
        if (invulnerable) return;

        currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);
        OnHealthChanged?.Invoke(currentHP, maxHP); 

        if (currentHP <= 0 && !isDead)
        {
            currentHP = 0;
            isDead = true;
            OnPlayerDied?.Invoke(); 
            HandleDeath();
        }
    }

    public void SetInvulnerable(bool value) => invulnerable = value;

    /// <summary>Se llama desde DashHandler cuando inicia un dash para arrancar el cooldown.</summary>
    public void RegisterDash() => lastDashTime = Time.time;
    #endregion

    #region Inputs & Movimiento
    private void HandleInputs()
    {
        if(PlayerInputs.Instance != null)
        {
         if (PlayerInputs.Instance.Jump()) Jump();
         if (PlayerInputs.Instance.Attack()) combatHandler.TryAttack();
        }
    }

    private void MovePlayer()
    {
        Vector2 input = PlayerInputs.Instance.GetMoveAxis(); // (x = horizontal, y = vertical)
        Vector3 targetDir = (orientation.forward * input.y + orientation.right * input.x).normalized;
        float targetSpeed = PlayerInputs.Instance.RunHeld() ? runSpeed : walkSpeed;

        Vector3 targetVelocity = targetDir * targetSpeed;
        Vector3 currentVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vector3 velocityChange = targetVelocity - currentVelocity;

        float accelRate = (targetDir.magnitude > 0.1f) ? acceleration : deceleration;
        float frictionBoost = (targetDir.magnitude == 0f) ? 1.5f : 1f;

        Vector3 force = velocityChange * accelRate * frictionBoost;

        const float maxForce = 50f;
        if (force.magnitude > maxForce) force = force.normalized * maxForce;
        force *= rb.mass;

        if (IsGrounded)
        {
            rb.AddForce(new Vector3(force.x, 0f, force.z), ForceMode.Force);
        }
        else
        {
            const float airControl = 0.5f;
            rb.AddForce(new Vector3(force.x, 0f, force.z) * airControl * airMultiplier, ForceMode.Force);
        }

        float targetDrag = IsGrounded ? groundDrag : 0f;
        rb.drag = Mathf.Lerp(rb.drag, targetDrag, Time.deltaTime * 10f);
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
        if (!IsGrounded) return;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    #endregion
    private IEnumerator InvokeEventInitializationPlayer()
    {
        yield return new WaitForSeconds(1);

        onPlayerInitialized?.Invoke(this);
    }
    #region Death & Respawn
    private void HandleDeath()
    {
        CanMove = false;
        rb.velocity = Vector3.zero;
        SetInvulnerable(true);
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // TODO: UIManager.Instance.ShowDeathScreen();
        yield return new WaitForSeconds(2f);

        // Reset stats
        currentHP = maxHP;
        isDead = false;
        SetInvulnerable(false);
        CanMove = true;

        // TODO:
        // DungeonManager.Instance.ReturnToLobby();
        // DungeonManager.Instance.ResetDungeon();
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        if (!showGroundGizmo) return;

        Gizmos.color = Color.green;
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * groundCheckDistance;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(end, 0.05f);
    }
    #endregion
}