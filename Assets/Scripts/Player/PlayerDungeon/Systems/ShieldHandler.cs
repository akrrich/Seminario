using UnityEngine;

public class ShieldHandler : MonoBehaviour
{
    [Header("Shield Settings")]
    [SerializeField] private float staminaCost = 25f;
    [SerializeField] private float cooldownDuration = 1.5f;
    [SerializeField] private GameObject shieldObject; // Visual del escudo (opcional)

    private PlayerStaminaManager stamina;
    private float lastShieldUseTime = -999f;
    private bool isActive;

    public bool IsActive => isActive;

    private void Awake()
    {
        stamina = GetComponent<PlayerStaminaManager>();
        if (shieldObject != null)
            shieldObject.SetActive(false);
    }

    public void TryUseShield()
    {
        if (isActive) return; // Ya est� activo
        if (Time.time < lastShieldUseTime + cooldownDuration) return; // Cooldown
        if (stamina == null || !stamina.CanUse(staminaCost)) return; // No hay stamina suficiente

        ActivateShield();
    }

    private void ActivateShield()
    {
        isActive = true;
        lastShieldUseTime = Time.time;
        stamina?.Use(staminaCost);

        if (shieldObject != null)
            shieldObject.SetActive(true);
    }

    public void DeactivateShield()
    {
        if (!isActive) return;

        isActive = false;
        if (shieldObject != null)
            shieldObject.SetActive(false);
    }

    // M�todo que los enemigos podr�an llamar para saber si bloquear el da�o
    public bool TryBlockAttack()
    {
        return isActive;
    }


    private void Update()
    {
        // Desactivar si el jugador suelta el bot�n del escudo
        if (isActive && (PlayerInputs.Instance == null || !PlayerInputs.Instance.Shield()))
        {
            DeactivateShield();
        }
    }
}
