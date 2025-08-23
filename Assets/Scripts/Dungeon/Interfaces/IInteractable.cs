using TMPro;

public enum InteractionMode
{
    Press, Hold     
}

public interface IInteractable 
{
    InteractionMode InteractionMode { get; }

    void Interact(bool isPressed); 

    void ShowOutline();

    void HideOutline();

    void ShowMessage(TextMeshProUGUI interactionManagerUIText);

    void HideMessage(TextMeshProUGUI interactionManagerUIText); 
}
