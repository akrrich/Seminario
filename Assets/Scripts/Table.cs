using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class Table : MonoBehaviour, IInteractable
{
    private PlayerController playerController;
    private Outline outline;

    private Table auxiliarTable;
    private ClientView auxiliarClientView;

    private GameObject chair;
    private GameObject dish;
    private GameObject dirty;

    private NavMeshObstacle[] navMeshObstacle;

    private List<Transform> dishPositions = new List<Transform>(); // Representa las posiciones hijas del plato

    private List<Food> currentFoods = new List<Food>();

    private float currentCleanProgress = 0f;

    private bool isOccupied = false;
    private bool isDirty = false;

    public Transform ChairPosition { get => chair.transform; }
    public Transform DishPosition { get => dish.transform; } // Solamente para que mire hacia adelante que es esta posicion

    public List<Transform> DishPositions { get => dishPositions; }

    public List<Food> CurrentFoods { get => currentFoods; set => currentFoods = value; }

    public InteractionMode InteractionMode 
    {
        get
        {
            if (isDirty)
            {
                return InteractionMode.Hold;
            }

            else
            {
                return InteractionMode.Press;
            }
        }
    }

    public float CurrentCleanProgress { get => currentCleanProgress; set => currentCleanProgress = value; }
    
    public bool IsOccupied { get => isOccupied; set => isOccupied = value; }
    public bool IsDirty { get => isDirty; }


    void Awake()
    {
        FindObjectsAndComponents();
    }


    public void Interact(bool isPressed)
    {
        if (isDirty)
        {
            if (isPressed)
            {
                PlayerController.OnCleanDirtyTableIncreaseSlider?.Invoke(this);
            }

            else
            {
                PlayerController.OnCleanDirtyTableDecreaseSlider?.Invoke(this);
            }

            return;
        }

        if (auxiliarClientView != null && auxiliarClientView.CanTakeOrder)
        {
            PlayerController.OnTakeOrder?.Invoke();
            return;
        }

        PlayerController.OnHandOverFood?.Invoke();
        return;
    }

    public void ShowOutline()
    {
        if (isDirty)
        {
            outline.OutlineWidth = 5f;
            InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Interactive);

            PlayerView.OnActivateSliderCleanDirtyTable?.Invoke();
            return;
        }

        if (auxiliarTable == null || auxiliarTable != this)
        {
            auxiliarTable = this;
            auxiliarClientView = GetComponentInChildren<ClientView>();
        }

        // Si hay un cliente sentado
        if (ChairPosition.childCount > 0 && auxiliarClientView != null)
        {
            // Tomar pedido
            if (isOccupied && auxiliarClientView.ReturnSpriteWaitingToBeAttendedIsActive())
            {
                if (!auxiliarClientView.CanTakeOrder)
                {
                    outline.OutlineWidth = 5f;
                    InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Interactive);

                    auxiliarClientView.CanTakeOrder = true;
                    PlayerController.OnTableCollisionEnterForTakeOrder?.Invoke(this);
                }
            }

            // Entregar pedido
            bool hasChildren = false;
            foreach (Transform child in playerController.PlayerView.Dish.transform)
            {
                if (child.childCount > 0)
                {
                    hasChildren = true;
                    break;
                }
            }

            if (hasChildren && isOccupied && auxiliarClientView.ReturnSpriteFoodIsActive())
            {
                outline.OutlineWidth = 5f;
                InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Interactive);

                PlayerController.OnTableCollisionEnterForHandOverFood?.Invoke(this);
            }
        }

        else
        {
            // El cliente se fue
            if (auxiliarClientView != null)
            {
                auxiliarClientView.CanTakeOrder = false;
            }

            PlayerController.OnTableCollisionExitForTakeOrder?.Invoke();
            PlayerController.OnTableCollisionExitForHandOverFood?.Invoke();

            auxiliarTable = null;
            auxiliarClientView = null;
        }
    }

    public void HideOutline()
    {
        outline.OutlineWidth = 0f;
        InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Normal);

        PlayerView.OnDeactivateSliderCleanDirtyTable?.Invoke();

        if (ChairPosition.childCount > 0) // Si tiene a alguien sentado
        {
            ClientView clientView = gameObject.GetComponentInChildren<ClientView>();
            clientView.CanTakeOrder = false;

            PlayerController.OnTableCollisionExitForTakeOrder?.Invoke();
            PlayerController.OnTableCollisionExitForHandOverFood?.Invoke();

            auxiliarTable = null;
            auxiliarClientView = null;
            return;
        }
    }

    public void ShowMessage(TMPro.TextMeshProUGUI interactionManagerUIText)
    {
        string keyText = $"<color=yellow> {PlayerInputs.Instance.GetInteractInput()} </color>";

        if (isDirty)
        {
            interactionManagerUIText.text = $"Hold" + keyText + "to clean the table";
            return;
        }

        // Si hay un cliente sentado
        if (ChairPosition.childCount > 0 && auxiliarClientView != null)
        {
            // Tomar pedido
            if (isOccupied && auxiliarClientView.ReturnSpriteWaitingToBeAttendedIsActive())
            {
                if (auxiliarClientView.CanTakeOrder)
                {
                    interactionManagerUIText.text = $"Press" + keyText + "to take order";
                }
            }

            // Entregar pedido
            bool hasChildren = false;
            foreach (Transform child in playerController.PlayerView.Dish.transform)
            {
                if (child.childCount > 0)
                {
                    hasChildren = true;
                    break;
                }
            }

            if (hasChildren && isOccupied && auxiliarClientView.ReturnSpriteFoodIsActive())
            {
                interactionManagerUIText.text = $"Press" + keyText + "to deliver the order";
            }
        }
    }

    public void HideMessage(TMPro.TextMeshProUGUI interactionManagerUIText)
    {
        interactionManagerUIText.text = string.Empty;
    }

    public void SetDirty(bool current)
    {
        isDirty = current;
        dirty.SetActive(current);
    }

    /// <summary>
    /// Analizar el metodo por el tema de el NavMesh del NPC
    /// </summary>
    public void SetNavMeshObstacles(bool current)
    {
        for (int i = 0; i < navMeshObstacle.Length; i++)
        {
            // No ejecutar si ya estaba activado y current es true, esto sirve por si se fue de la cola de espera porque no se libero ninguna silla
            if (navMeshObstacle[i].isActiveAndEnabled && current)
            {
                continue;
            }

            navMeshObstacle[i].enabled = current;
        }
    }


    private void FindObjectsAndComponents()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        outline = GetComponentInChildren<Outline>();

        chair = transform.Find("Chair").gameObject;
        dish = transform.Find("Dish").gameObject;
        dirty = transform.Find("Dirty").gameObject;

        navMeshObstacle = GetComponentsInChildren<NavMeshObstacle>();

        foreach (Transform childs in dish.transform)
        {
            dishPositions.Add(childs.GetComponent<Transform>());
        }
    }
}
