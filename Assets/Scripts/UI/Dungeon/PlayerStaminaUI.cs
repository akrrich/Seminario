using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStaminaUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private float duration = 0.3f; // Duraci�n de la transici�n

    private Coroutine staminaCoroutine;
    private void Awake()
    {
        // Suscribirse al evento est�tico
        PlayerDungeonHUD.OnStaminaChanged += UpdateStaminaUI;
    }

    private void OnDestroy()
    {
        // Desuscribirse del evento est�tico
        PlayerDungeonHUD.OnStaminaChanged -= UpdateStaminaUI;
    }

    private void UpdateStaminaUI(float currentHP, float maxHP)
    {
        float targetFill = Mathf.Clamp01(currentHP / maxHP);

        if (staminaCoroutine != null)
            StopCoroutine(staminaCoroutine);

        staminaCoroutine = StartCoroutine(LerpStaminaFill(targetFill));
    }

    private IEnumerator LerpStaminaFill(float target)
    {
        float start = fillImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fillImage.fillAmount = Mathf.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        fillImage.fillAmount = target; 

    }
}
