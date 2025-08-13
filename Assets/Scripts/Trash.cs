using UnityEngine;

public class Trash : MonoBehaviour, IInteractable
{
    private PlayerController playerController;
    private Outline outline;


    void Awake()
    {
        GetComponents();
    }


    public void Interact()
    {
        PlayerController.OnThrowFoodToTrash?.Invoke();
        HideOutline();
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


    private void GetComponents()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        outline = GetComponent<Outline>();
    }
}
