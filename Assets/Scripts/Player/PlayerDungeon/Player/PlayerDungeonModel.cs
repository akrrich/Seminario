using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerDungeonModel : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float speed;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float dashCooldown = 1.5f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool showGroundGizmo = true;

    private float currentHP;
    private float lastDashTime = -Mathf.Infinity;

    private Rigidbody rb;
    private PlayerPhase currentPhase = PlayerPhase.Idle;

    public static event Action<PlayerDungeonModel> OnPlayerInitialized;
    public event Action<PlayerPhase> OnPhaseChanged;

    // === Properties ===
    public float Speed { get => speed; set => speed = value; }
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
    public float JumpForce => jumpForce;
    public float DashCooldown => dashCooldown;
    public bool CanDash => Time.time >= lastDashTime + dashCooldown;

    public Rigidbody Rb => rb;
    public float CurrentHP => currentHP;
    public bool IsDead => currentPhase == PlayerPhase.Dead;
    public bool IsGrounded => Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    public bool IsFalling => rb.velocity.y < -0.1f && !IsGrounded;
    public PlayerPhase CurrentPhase => currentPhase;

    public Vector3 MoveDirection { get; set; }
    public bool IsInvulnerable { get; private set; }
    public bool CanMove { get; set; } = true;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        currentHP = maxHP;
        OnPlayerInitialized?.Invoke(this);
    }
    private void FixedUpdate()
    {
        if (CanMove && MoveDirection.sqrMagnitude > 0.01f)
        {
            Vector3 velocity = MoveDirection * Speed;
            velocity.y = rb.velocity.y;
            rb.velocity = velocity;
        }
    }
    public void Jump()
    {
        if (IsGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    public void LookAt(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }
    public void TakeDamage(int amount)
    {
        if (IsInvulnerable || IsDead) return;

        currentHP -= amount;
        if (currentHP <= 0)
        {
            currentHP = 0;
            RequestPhaseChange(PlayerPhase.Dead);
        }
    }
    public void SetInvulnerable(bool state) => IsInvulnerable = state;
    public void RegisterDash() => lastDashTime = Time.time;

    public void RequestPhaseChange(PlayerPhase next)
    {
        if (currentPhase == next || IsDead) return;

        currentPhase = next;
        OnPhaseChanged?.Invoke(currentPhase);
    }
    private void OnDrawGizmosSelected()
    {
        if (!showGroundGizmo) return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
