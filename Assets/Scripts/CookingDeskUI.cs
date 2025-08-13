using UnityEngine;

public class CookingDeskUI : MonoBehaviour, IInteractable
{
    private PlayerController playerController;
    private Outline outline;


    void Awake()
    {
        GetComponents();
    }


    public void Interact()
    {
        // NADA, lo maneja la maquina de estados
    }

    public void ShowOutline()
    {
        playerController.PlayerModel.IsCollidingCookingDeskUI = true;
        PlayerView.OnCollisionEnterWithCookingDeskUIForCookModeMessage?.Invoke();

        outline.OutlineWidth = 5f;
        InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Interactive);
    }

    public void HideOutline()
    {
        playerController.PlayerModel.IsCollidingCookingDeskUI = false;
        PlayerView.OnCollisionExitWithCookingDeskUIForCookModeMessage?.Invoke();

        outline.OutlineWidth = 0f;
        InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Normal);
    }


    private void GetComponents()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        outline = GetComponent<Outline>();
    }
}
