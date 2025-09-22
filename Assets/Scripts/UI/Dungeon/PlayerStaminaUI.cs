using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fillImage;
    private void OnEnable()
    {
        // Suscribirse al evento est�tico
        PlayerDungeonHUD.OnStaminaChanged += UpdateStaminaUI;
    }

    private void OnDisable()
    {
        // Desuscribirse del evento est�tico
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
