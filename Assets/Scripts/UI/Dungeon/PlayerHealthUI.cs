using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private void OnEnable()
    {
        PlayerDungeonHUD.OnHealthChanged += UpdateBar;
    }

    private void OnDisable()
    {
        PlayerDungeonHUD.OnHealthChanged -= UpdateBar;
    }

    private void UpdateBar(float current, float max)
    {
        fillImage.fillAmount = current / max;
    }
}
