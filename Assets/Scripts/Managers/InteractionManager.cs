using UnityEngine;

public class InteractionManager : Singleton<InteractionManager>
{
    [SerializeField] private InteractionManagerData interactionManagerData;

    private IInteractable currentTarget;
    private IInteractable previousTarget;


    void Awake()
    {
        CreateSingleton(true);
        SuscribeToUpdateManagerEvent();
    }

    // Simulacion de Update
    void UpdateInteractionManager()
    {
        DetectTarget();
        InteractWithTarget();
    }


    private void SuscribeToUpdateManagerEvent()
    {
        UpdateManager.OnUpdate += UpdateInteractionManager;
    }

    private void DetectTarget()
    {
        if (InteractionManagerUI.Instance == null) return;
        if (!InteractionManagerUI.Instance.CenterPointUI.gameObject.activeSelf) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactionManagerData.InteractionDistance, LayerMask.GetMask("Interactable")))
        {
            IInteractable hitTarget = hit.collider.GetComponent<IInteractable>()?? hit.collider.GetComponentInChildren<IInteractable>()?? hit.collider.GetComponentInParent<IInteractable>();

            if (hitTarget != null)
            {
                if (hitTarget != previousTarget && previousTarget != null)
                {
                    previousTarget.HideOutline();
                }

                currentTarget = hitTarget;
                previousTarget = hitTarget;

                // Mostrar outline siempre, aunque sea el mismo objeto
                currentTarget.ShowOutline();

                return;
            }
        }

        // Si no hay target y antes hab�a uno, limpiamos
        if (previousTarget != null)
        {
            previousTarget.HideOutline();
            previousTarget = null;
            currentTarget = null;
        }
    }

    private void InteractWithTarget()
    {
        if (InteractionManagerUI.Instance == null) return;
        if (!InteractionManagerUI.Instance.CenterPointUI.gameObject.activeSelf) return;

        if (currentTarget != null && !PauseManager.Instance.IsGamePaused)
        {
            switch (currentTarget.InteractionMode)
            {
                case InteractionMode.Press:
                    if (PlayerInputs.Instance.InteractPress())
                    {
                        currentTarget.Interact(true);
                    }
                    break;

                case InteractionMode.Hold:
                    if (PlayerInputs.Instance.InteractHold())
                    {
                        currentTarget.Interact(true);
                    }

                    else
                    {
                        currentTarget.Interact(false);
                    }
                    break;
            }
        }
    }
}
