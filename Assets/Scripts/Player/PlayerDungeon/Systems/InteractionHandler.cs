using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactableLayer;
    private IInteractable currentInteractable;

    void Update()
    {
        DetectInteractable();

        if (PlayerInputs.Instance.Interact() && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    void DetectInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange, interactableLayer);
        currentInteractable = null;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IInteractable interactable))
            {
                currentInteractable = interactable;
                // Mostrar mensaje de interacción si se desea
                break;
            }
        }
    }
}
