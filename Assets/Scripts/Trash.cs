using UnityEngine;
using System.Collections;
using TMPro;

public class Trash : MonoBehaviour, IInteractable
{
    private PlayerController playerController;

    public InteractionMode InteractionMode { get => InteractionMode.Press; }


    void Awake()
    {
        GetComponents();
        StartCoroutine(RegisterOutline());
    }

    void OnDestroy()
    {
        OutlineManager.Instance.Unregister(gameObject);
    }


    public void Interact(bool isPressed)
    {
        PlayerController.OnThrowFoodToTrash?.Invoke();
    }

    public void ShowOutline()
    {
        foreach (Transform child in playerController.PlayerView.Dish.transform)
        {
            // Verifica que las posiciones de la bandeja tengan hijos (COMIDAS)
            if (child.childCount > 0)
            {
                OutlineManager.Instance.ShowWithDefaultColor(gameObject);
                InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Interactive);
            }
        }
    }

    public void HideOutline()
    {
        OutlineManager.Instance.Hide(gameObject);
        InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Normal);
    }

    public void ShowMessage(TextMeshProUGUI interactionManagerUIText)
    {
        foreach (Transform child in playerController.PlayerView.Dish.transform)
        {
            if (child.childCount > 0)
            {
                string keyText = $"<color=yellow> {PlayerInputs.Instance.GetInteractInput()} </color>";
                interactionManagerUIText.text = $"Press" + keyText + "to throw food in the trash";
            }
        }
    }

    public void HideMessage(TextMeshProUGUI interactionManagerUIText)
    {
        interactionManagerUIText.text = string.Empty;
    }


    private void GetComponents()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private IEnumerator RegisterOutline()
    {
        yield return new WaitUntil(() => OutlineManager.Instance != null);

        OutlineManager.Instance.Register(gameObject);
    }
}
