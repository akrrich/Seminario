using UnityEngine;
using System.Collections;
using TMPro;

public class CookingDeskUI : MonoBehaviour, IInteractable
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
        playerController.PlayerModel.IsCooking = true;
    }

    public void ShowOutline()
    {
        OutlineManager.Instance.ShowWithDefaultColor(gameObject);
        InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Interactive);
    }

    public void HideOutline()
    {
        OutlineManager.Instance.Hide(gameObject);
        InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Normal);
    }

    public void ShowMessage(TextMeshProUGUI interactionManagerUIText)
    {
        string keyText = $"<color=yellow> {PlayerInputs.Instance.GetInteractInput()} </color>";
        interactionManagerUIText.text = $"Press" + keyText + "to start cooking";
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
