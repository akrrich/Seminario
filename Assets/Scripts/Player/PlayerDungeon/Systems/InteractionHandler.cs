

using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    [Header("Interaction variables")]
    [SerializeField] private float range = 5f;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField,Tooltip("El Punto de vista o donde apunta")]
    private Transform rayOrigin; 

    private IInteractable current;
    private IInteractable lastCurrent;

    
    private void Update()
    {
        Detect();

        if (PlayerInputs.Instance.Interact() && current != null)
            current.Interact();
    }

    private void Detect()
    {
        lastCurrent = current;
        current = null;

        // SphereCast en la dirección de la vista
        if (Physics.SphereCast(rayOrigin.position, radius, rayOrigin.forward,
                               out RaycastHit hit, range, interactableLayer))
        {
            Debug.Log("Tire esfera");
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                Debug.Log("Le di a un interactable");
                current = interactable;
            }
        }

        // Solo actualizar la UI si cambia el objeto bajo la mira
        if (current != lastCurrent)
        {
            if (current != null)
            {
                // Mostrar el prompt de interacción
                // UIManager.Instance.ShowInteractionPrompt(current); // ejemplo
            }
            else
            {
                // Ocultar el prompt de interacción
                // UIManager.Instance.HideInteractionPrompt(); // ejemplo
            }
        }
    }
}
