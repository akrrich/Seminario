using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fillImage;
    private void OnEnable()
    {
        // Suscribirse al evento estático
        PlayerDungeonHUD.OnStaminaChanged += UpdateStaminaUI;
    }

    private void OnDisable()
    {
        // Desuscribirse del evento estático
        PlayerDungeonHUD.OnStaminaChanged -= UpdateStaminaUI;
    }

    private void UpdateStaminaUI(float currentStamina, float maxStamina)
    {
        float staminaPercentage = currentStamina / maxStamina;
        if (fillImage != null)
        {
            fillImage.fillAmount = staminaPercentage;
        }
    }
}
