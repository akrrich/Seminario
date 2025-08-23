using System;
using System.Collections;
using UnityEngine;

public class PlayerDungeonModel : MonoBehaviour, IDamageable
{
    #region Inspector Fields
    [Header("Stats")]
    [Header("Stamina")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 10f; // por segundo
    [SerializeField] private float staminaRegenDelay = 1f; // pausa antes de regenerar tras gastar

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

    [Header("Combat")]
    [SerializeField] private int currentWeaponDamage = 5;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool showGroundGizmo = true;
    #endregion

    #region Private Fields
    private PlayerHealth playerHealth;
    private float currentStamina;

    private bool invulnerable = false;
    private float lastStaminaUseTime;

    private Vector3 moveDirection;
    private Transform orientation;
    private Rigidbody rb;
    private CombatHandler combatHandler;
    #endregion

    #region Properties
    // -------- Estado general --------
    public bool IsInvulnerable => invulnerable;
    public bool CanMove { get; set; } = true;

    // -------- Stamina --------
    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;
    public bool HasStamina(float amount) => currentStamina >= amount;

    // -------- Movimiento --------
    public float Speed { get => speed; set => speed = value; }
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
    public float JumpForce => jumpForce;
    public Rigidbody Rb => rb;
    public Vector3 MoveDirection { get => moveDirection; set => moveDirection = value; }

    public bool IsGrounded => Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    public bool IsFalling => rb.velocity.y < -0.1f && !IsGrounded;

    // -------- Combate --------
    public int CurrentWeaponDamage { get => currentWeaponDamage; set => currentWeaponDamage = value; }
    public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }

    //------ Eventos ------
    //--UI--
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnStaminaChanged;
    
   
    public static Action<PlayerDungeonModel> onPlayerInitialized;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerHealth = GetComponent<PlayerHealth>();
        combatHandler = GetComponent<CombatHandler>();
        orientation = transform.Find("Orientation");

        rb.freezeRotation = true;

        playerHealth.OnHealthChanged += (current, max) => OnHealthChanged?.Invoke(current, max);
        playerHealth.OnPlayerDied += HandleDeath;

        currentStamina = maxStamina;
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);

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

        RegenerateStamina();
    }
    #endregion

    #region Public Methods
    // --------- Vida ---------
    public void TakeDamage(int amount)
    {
        playerHealth.TakeDamage(amount);
    }

    public void SetInvulnerable(bool value) => invulnerable = value;

    // --------- Stamina ---------
    public void UseStamina(float amount)
    {
        if (amount <= 0f) return;

        currentStamina = Mathf.Clamp(currentStamina - amount, 0, maxStamina);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);

        lastStaminaUseTime = Time.time;
    }

    private void RegenerateStamina()
    {
        if (currentStamina >= maxStamina) return;
        if (Time.time < lastStaminaUseTime + staminaRegenDelay) return;

        currentStamina += staminaRegenRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }
    #endregion

    #region Inputs & Movimiento
    private void HandleInputs()
    {
        if (PlayerInputs.Instance == null) return;

        if (PlayerInputs.Instance.Jump())
            Jump();

        if (PlayerInputs.Instance.Shield())
        {
            if (!combatHandler.IsAttacking)
            {
                combatHandler.TryUseShield(); 
            }
        }
        else 
        {
            if (PlayerInputs.Instance.Attack())
            {
                if (!combatHandler.IsShieldActive) 
                {
                    combatHandler.TryAttack();
                }
            }
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
        playerHealth.SetInvulnerable(true);
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(1f);

        DungeonManager.Instance.OnPlayerDeath();

        playerHealth.Revive();
        CanMove = true;

        Debug.Log("Jugador respawneado en la dungeon");
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
