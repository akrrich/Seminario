using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image healthFillImage;

    private void OnEnable()
    {
        // Suscribirse al evento estático del HUD
        PlayerDungeonHUD.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        PlayerDungeonHUD.OnHealthChanged -= UpdateHealthUI;
    }

    private void UpdateHealthUI(float currentHP, float maxHP)
    {
        float healthPercentage = Mathf.Clamp01(currentHP / maxHP);

        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = healthPercentage;
        }
    }
}
