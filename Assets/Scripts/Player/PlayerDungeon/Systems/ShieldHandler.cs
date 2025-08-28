using UnityEngine;

public class ShieldHandler : MonoBehaviour
{
    [Header("Shield Settings")]
    [SerializeField] private float staminaCost = 25f;
    [SerializeField] private float cooldownDuration = 1.5f;
    [SerializeField] private GameObject shieldObject; // Visual del escudo (opcional)
    [SerializeField] private bool isActive;
    [Header("Behaviour")]
    [SerializeField] private bool holdToBlock = true;   // true = mantener; false = toggle
    [SerializeField] private bool debugLogs = true;

    private PlayerStamina stamina;
    private float lastShieldUseTime = -999f;

    public bool IsActive => isActive;

    private void Awake()
    {
        if (!stamina) stamina = GetComponent<PlayerStamina>();
        if (!stamina) stamina = GetComponentInParent<PlayerStamina>();
        if (!stamina) stamina = GetComponentInChildren<PlayerStamina>(); // opcional
        if (shieldObject != null)
            shieldObject.SetActive(false);

        if (debugLogs)
            Debug.Log($"[Shield] Awake. shieldObject={(shieldObject ? shieldObject.name : "null")}");
    }

    public void TryUseShield()
    {
        // En modo HOLD: si ya está activo, no volvemos a activarlo.
        if (holdToBlock && isActive)
        {
            if (debugLogs) Debug.Log("[Shield] Ya activo (hold)");
            return;
        }

        // Cooldown
        if (Time.time < lastShieldUseTime + cooldownDuration)
        {
            if (debugLogs) Debug.Log("[Shield] En cooldown");
            return;
        }

        // Stamina
        if (stamina == null)
        {
            if (debugLogs) Debug.LogWarning("[Shield] No hay PlayerStaminaManager en este GameObject.");
            return;
        }
        if (!stamina.CanUse(staminaCost))
        {
            if (debugLogs) Debug.Log("[Shield] Stamina insuficiente");
            return;
        }

        // Modo TOGGLE: si está activo y hacemos click, lo apagamos (sin coste extra)
        if (!holdToBlock && isActive)
        {
            if (debugLogs) Debug.Log("[Shield] Toggle -> Desactivar");
            DeactivateShield();
            return;
        }

        // Activar
        ActivateShield();
    }

    private void ActivateShield()
    {
        isActive = true;
        lastShieldUseTime = Time.time;
        stamina.Use(staminaCost);

        if (shieldObject != null)
            shieldObject.SetActive(true);

        if (debugLogs) Debug.Log("[Shield] ACTIVADO (stamina -25)");
    }

    public void DeactivateShield()
    {
        if (!isActive) return;

        isActive = false;
        if (shieldObject != null)
            shieldObject.SetActive(false);

        if (debugLogs) Debug.Log("[Shield] DESACTIVADO");
    }

    private void Update()
    {
        // En modo HOLD, bajamos el escudo al soltar el botón
        if (holdToBlock && isActive && (PlayerInputs.Instance == null || !PlayerInputs.Instance.Shield()))
        {
            DeactivateShield();
        }
    }
}
