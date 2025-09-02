using TMPro;
using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [SerializeField] private InteractionMode interactionMode = InteractionMode.Press;
    [SerializeField] private string interactionMessage = "Teleport to Lobby";

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI interactionUIText;

    private bool isPlayerInRange = false;

    public InteractionMode InteractionMode => interactionMode;

    private void Awake()
    {
        StartCoroutine(RegisterOutline());
    }

    private void OnDestroy()
    {
        if (OutlineManager.Instance != null)
        {
            OutlineManager.Instance.Unregister(gameObject);
        }
    }
    public void Interact(bool isPressed)
    {
        TeleportToLobby();
    }

    private void TeleportToLobby()
    {
        Debug.Log($"[Teleport] Teleporting player to lobby from {gameObject.name}");

        if (DungeonManager.Instance != null)
        {
            DungeonManager.Instance.TeleportToLobby();
        }
        else
        {
            Debug.LogWarning("[Teleport] DungeonManager instance not found!");
        }
    }

    public void ShowOutline()
    {
        if (OutlineManager.Instance != null)
        {
            OutlineManager.Instance.Show(gameObject);
        }
    }

    public void HideOutline()
    {
        if (OutlineManager.Instance != null)
        {
            OutlineManager.Instance.Hide(gameObject);
        }
    }

    public void ShowMessage(TextMeshProUGUI interactionManagerUIText)
    {
        if (interactionManagerUIText != null)
        {
            string keyText = $"<color=yellow> {PlayerInputs.Instance.GetInteractInput()} </color>";
            interactionManagerUIText.text = $"Press" + keyText + $"to {interactionMessage.ToLower()}";
            interactionManagerUIText.gameObject.SetActive(true);
        }
    }

    public void HideMessage(TextMeshProUGUI interactionManagerUIText)
    {
        if (interactionManagerUIText != null)
        {
            interactionManagerUIText.text = string.Empty;
            interactionManagerUIText.gameObject.SetActive(false);
        }
    }

    private IEnumerator RegisterOutline()
    {
        yield return new WaitUntil(()=>OutlineManager.Instance != null);
        OutlineManager.Instance.Register(this.gameObject);  
    }
}