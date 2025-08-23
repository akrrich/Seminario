using UnityEngine;

public class CookingDeskUI : MonoBehaviour, IInteractable
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
        // NADA, lo maneja la maquina de estados
    }

    public void ShowOutline()
    {
        playerController.PlayerModel.IsCollidingCookingDeskUI = true;

        outline.OutlineWidth = 5f;
        InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Interactive);
    }

    public void HideOutline()
    {
        playerController.PlayerModel.IsCollidingCookingDeskUI = false;

        outline.OutlineWidth = 0f;
        InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Normal);
    }

    public void ShowMessage(TMPro.TextMeshProUGUI interactionManagerUIText)
    {
        string keyText = $"<color=yellow> {PlayerInputs.Instance.GetInteractInput()} </color>";
        interactionManagerUIText.text = $"Press" + keyText + "to start cooking";
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
