
using UnityEngine;

public class SafetyPoint : MonoBehaviour
{
    [Header("Fall Detection")]
    [SerializeField] private float fallThreshold = -10f;
    [SerializeField] private float saveFrequency = 0.5f;

    private PlayerDungeonModel playerRef;
    private PlayerHealth playerHealth;
    private Rigidbody rb;

    private Vector3 lastSafePosition;
    private bool hasBeenTeleported = false;
    private float nextSaveTime;
    private void Awake()
    {
        playerRef = GetComponent<PlayerDungeonModel>();
        playerHealth = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody>();

        lastSafePosition = transform.position;
        nextSaveTime = Time.time;
    }
    private void Update()
    {
        FallCheck();
    }
    public void TeleportInitiate(int damageAmount)
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
            ExecuteTeleport();
        }
        else
        {
            Debug.LogError("PlayerHealth component not found on the player.");
        }
    }
    public void ExecuteTeleport()
    {
        if (playerRef != null && playerRef.Rb != null)
        {
            playerRef.Rb.velocity = Vector3.zero;
            playerRef.Rb.angularVelocity = Vector3.zero;
        }
        transform.position = lastSafePosition;
        Debug.Log($"Teleported to last safe point:{lastSafePosition}");
    }
    public Vector3 GetLastSafePosition()
    {
        return lastSafePosition;
    }
    private void FallCheck()
    {
        // Fail-safe check
        if (playerRef == null || playerHealth == null) return;

        // 1. Detecci�n de Ca�da Fatal (Vac�o)
        if (transform.position.y < fallThreshold && !hasBeenTeleported)
        {
            int lethalDamage = playerHealth.MaxHP + 1; // Ajusta a tu propiedad real MaxHealth
            TeleportInitiate(lethalDamage);
            hasBeenTeleported = true; // Previene m�ltiples llamadas por una misma ca�da
        }

        // 2. Guardar la Posici�n Segura (Optimizado con temporizador)
        if (Time.time >= nextSaveTime && playerRef.IsGrounded)
        {
            lastSafePosition = transform.position;
            hasBeenTeleported = false; // Permite el teletransporte si vuelve a caer
            nextSaveTime = Time.time + saveFrequency;
        }
    }
}
