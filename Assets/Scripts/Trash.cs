using UnityEngine;

public class Trash : MonoBehaviour, IInteractable
{
    private PlayerController playerController;
    private Outline outline;

    public InteractionMode InteractionMode { get => InteractionMode.Press; }


    void Awake()
    {
        GetComponents();
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
                PlayerView.OnCollisionEnterWithTrashForTrashModeMessage?.Invoke();

                outline.OutlineWidth = 5f;
                InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Interactive);
            }
        }
    }

    public void HideOutline()
    {
        PlayerView.OnCollisionExitWithTrashForTrashModeMessage?.Invoke();

        outline.OutlineWidth = 0f;
        InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Normal);
    }

    public void ShowMessage(TMPro.TextMeshProUGUI interactionManagerUIText)
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

    public void HideMessage(TMPro.TextMeshProUGUI interactionManagerUIText)
    {
        interactionManagerUIText.text = string.Empty;
    }


    private void GetComponents()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        outline = GetComponent<Outline>();
    }
}
