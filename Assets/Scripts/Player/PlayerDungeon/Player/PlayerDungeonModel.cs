using System.Collections;
using UnityEngine;
public class PlayerDungeonModel : MonoBehaviour, IDamageable
{
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
    [SerializeField] private float groundDrag=5f;
    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;
    private float dashCooldown = 1.5f;
    [Header("Combat")]
    [SerializeField] private int currentWeaponDamage = 5;
    [SerializeField] private float attackCooldown = 1f;

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
    public float CurrentHP => currentHP;
    //===WEAPON===
    public int CurrentWeaponDamage {get=> currentWeaponDamage; set=> currentWeaponDamage = value; }
    public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
    public Rigidbody Rb => rb;
    public Vector3 MoveDirection { get=> _moveDirection; set=>_moveDirection=value; }
    public bool CanDash => Time.time >= lastDashTime + dashCooldown;
    public bool IsDead => isDead;
    public bool IsGrounded => Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    public bool IsFalling => rb.velocity.y < -0.1f && !IsGrounded;
    public bool IsInvulnerable { get; private set; }
    public bool CanMove { get; set; } = true;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        dash = GetComponent<DashHandler>();
        combat = GetComponent<CombatHandler>();
        orientation = transform.Find("Orientation");
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
            HandleDeath();
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
    public void MovePlayer()
    {
        // INPUT DE MOVIMIENTO
        Vector2 input = PlayerInputs.Instance.GetMoveAxis(); // (horizontal, vertical)
        Vector3 targetDirection = (orientation.forward * input.y + orientation.right * input.x).normalized;

        // VELOCIDADES
        float targetSpeed = PlayerInputs.Instance.RunHeld() ? runSpeed : walkSpeed;
        Vector3 targetVelocity = targetDirection * targetSpeed;
        Vector3 currentVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // CALCULO DE CAMBIO DE VELOCIDAD
        Vector3 velocityChange = targetVelocity - currentVelocity;

        // ACELERACIÓN O DESACELERACIÓN SEGÚN HAYA INPUT
        float accelRate = (targetDirection.magnitude > 0.1f) ? acceleration : deceleration;

        // EXTRA FRENADO AL SOLTAR LOS CONTROLES
        float frictionBoost = (targetDirection.magnitude == 0f) ? 1.5f : 1f;

        // FUERZA TOTAL CALCULADA
        Vector3 force = velocityChange * accelRate * frictionBoost;

        // LIMITAR FUERZA MÁXIMA PARA SUAVIDAD
        float maxForce = 50f;
        if (force.magnitude > maxForce)
            force = force.normalized * maxForce;

        // MULTIPLICAR POR MASA
        force *= rb.mass;

        // APLICAR FUERZA
        if (IsGrounded)
        {
            rb.AddForce(new Vector3(force.x, 0f, force.z), ForceMode.Force);
        }
        else
        {
            // CONTROL EN EL AIRE (LIMITADO)
            float airControlFactor = 0.5f; // control parcial en el aire
            rb.AddForce(new Vector3(force.x, 0f, force.z) * airControlFactor * airMultiplier, ForceMode.Force);
        }

        // INTERPOLAR DRAG (SUAVIZA FRENO)
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
    private void HandleDeath()
    {
        CanMove = false;
        rb.velocity = Vector3.zero;
        SetInvulnerable(true);
        //        IngredientInventoryManager.Instance      ====== Aca iria el manejo de resetear el inventario temporal
        StartCoroutine(DeathSequence());
    }
    private IEnumerator DeathSequence()
    {
        // Mostrar pantalla de muerte 
        //UIManager.Instance.ShowDeathScreen(); 

        yield return new WaitForSeconds(2f);

        // 1. Reiniciar estado del jugador
        currentHP = maxHP;
        isDead = false;
        SetInvulnerable(false);
        CanMove = true;
        /*
        // 2. Teleport al lobby
        DungeonManager.Instance.ReturnToLobby();

        // 3. Resetear el dungeon
        DungeonManager.Instance.ResetDungeon();
        */
    }
    private void OnDrawGizmosSelected()
    {
        if (!showGroundGizmo) return;

        Gizmos.color = Color.green;
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * groundCheckDistance;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(end, 0.05f); // pequeño punto para ver el final
    }
}
