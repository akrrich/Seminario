

using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    [SerializeField] private float range = 3f;
    [SerializeField] private LayerMask interactableLayer;

    private IInteractable current;
    private readonly Collider[] hits = new Collider[10];


    private void Update()
    {
        Detect();

        if (PlayerInputs.Instance.Interact() && current != null)
            current.Interact();
    }

    private void Detect()
    {
        current = null;
        int count = Physics.OverlapSphereNonAlloc(transform.position, range,
                                                  hits, interactableLayer);

        float best = float.MaxValue;
        for (int i = 0; i < count; i++)
        {
            if (hits[i].TryGetComponent(out IInteractable cand))
            {
                float d = Vector3.SqrMagnitude(hits[i].transform.position - transform.position);
                if (d < best)
                {
                    best = d;
                    current = cand;
                }
            }
        }

        // TODO: mostrar/ocultar prompt UI aquí con (current != null)
    }
}
