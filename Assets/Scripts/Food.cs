using System.Collections;
using UnityEngine;

public enum FoodType // FrutosDelBosqueOscuro, SopaDeLaLunaPlateada, CarneDeBestia, CarneCuradaDelAbismo, SusurroDelElixir
{
    DarkWoodBerries, SoupOfTheSilverMoon, BeastStew, AbyssCuredMeet, LastWhisperElixir
}

public enum CookingStates
{
    Raw,
    Cooked,
    Burned
}

public class Food : MonoBehaviour, IInteractable
{
    // No usar el metodo OnDisabled de Unity

    [SerializeField] private FoodData foodData;

    private CookingManager cookingManager;
    private Table currentTable; // Esta Table hace referencia a la mesa en la cual podemos entregar el pedido

    private Rigidbody rb;
    private BoxCollider boxCollider;
    private MeshRenderer meshRenderer;
    private Color originalColor;

    private Transform stovePosition;
    private Transform playerDishPosition;

    [SerializeField] private FoodType foodType;
    private CookingStates currentCookingState;

    private float cookTimeCounter = 0f;

    private bool isInstantiateFirstTime = true;
    private bool isInPlayerDishPosition = false;
    private bool isServedInTable = false;

    public InteractionMode InteractionMode { get => InteractionMode.Press; }

    public FoodType FoodType { get => foodType; }
    public CookingStates CurrentCookingState { get => currentCookingState; }


    void Awake()
    {
        SuscribeToPlayerControllerEvents();
        GetComponents();
        CoroutineHelper.Instance.StartHelperCoroutine(RegisterOutline());
        Initialize();
    }

    void OnEnable()
    {
        StartCoroutine(CookGameObject());
    }

    void OnDestroy()
    {
        UnsuscribeToPlayerControllerEvents();
        OutlineManager.Instance.Unregister(gameObject);
    }


    public void Interact(bool isPressed)
    {
        if (gameObject.activeSelf && !isServedInTable && !isInPlayerDishPosition && cookingManager.AvailableDishPositions.Count > 0)
        {
            PlayerView.OnEnabledDishForced?.Invoke(true);

            isInPlayerDishPosition = true;
            isServedInTable = true;

            cookingManager.ReleaseStovePosition(stovePosition);
            playerDishPosition = cookingManager.MoveFoodToDish(this);

            StartCoroutine(DisablePhysics());
        }
    }

    public void ShowOutline()
    {
        if (!isServedInTable)
        {
            OutlineManager.Instance.ShowWithDefaultColor(gameObject);
            InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Interactive);
        }
    }

    public void HideOutline()
    {
        OutlineManager.Instance.Hide(gameObject);
        InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Normal);
    }

    public void ReturnObjetToPool()
    {
        cookingManager.ReturnObjectToPool(foodType, this);
        RestartValues();
    }

    public void ShowMessage(TMPro.TextMeshProUGUI interactionManagerUIText)
    {
        if (!isServedInTable)
        {
            string keyText = $"<color=yellow> {PlayerInputs.Instance.GetInteractInput()} </color>";
            interactionManagerUIText.text = $"Press" + keyText + "to grab food";
        }
    }

    public void HideMessage(TMPro.TextMeshProUGUI interactionManagerUIText)
    {
        interactionManagerUIText.text = string.Empty;
    }


    private void SuscribeToPlayerControllerEvents()
    {
        PlayerController.OnHandOverFood += HandOver;
        PlayerController.OnThrowFoodToTrash += ThrowFoodToTrash;

        PlayerController.OnTableCollisionEnterForHandOverFood += SaveTable;
        PlayerController.OnTableCollisionExitForHandOverFood += ClearTable;
    }

    private void UnsuscribeToPlayerControllerEvents()
    {
        PlayerController.OnHandOverFood -= HandOver;
        PlayerController.OnThrowFoodToTrash -= ThrowFoodToTrash;

        PlayerController.OnTableCollisionEnterForHandOverFood -= SaveTable;
        PlayerController.OnTableCollisionExitForHandOverFood -= ClearTable;
    }

    private void GetComponents()
    {
        cookingManager = FindFirstObjectByType<CookingManager>();
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private IEnumerator RegisterOutline()
    {
        yield return new WaitUntil(() => OutlineManager.Instance != null);

        OutlineManager.Instance.Register(gameObject);
    }

    private void Initialize()
    {
        originalColor = meshRenderer.material.color;
        currentCookingState = CookingStates.Raw;
    }

    // Ejecutar unicamente la corrutina cuando se activa el objeto en caso de que se haya puesto en la hornalla
    private IEnumerator CookGameObject()
    {
        if (!isInPlayerDishPosition && !isInstantiateFirstTime)
        {
            stovePosition = cookingManager.CurrentStove;

            cookTimeCounter = 0f;

            while (cookTimeCounter <= foodData.TimeToBeenCooked + foodData.TimeToBeenBurned)
            {
                cookTimeCounter += Time.deltaTime;

                if (isInPlayerDishPosition)
                {
                    CheckCookingState();
                    yield break;
                }

                // Lerp del color original a un color oscuro
                Color targetColor = Color.Lerp(originalColor, Color.black, cookTimeCounter / (foodData.TimeToBeenCooked + foodData.TimeToBeenBurned));
                meshRenderer.material.color = targetColor;                

                yield return null;
            }

            CheckCookingState();
        }

        isInstantiateFirstTime = false;
    }

    private IEnumerator DisablePhysics()
    {
        yield return null; // Esperar un frame
        rb.isKinematic = true;
        boxCollider.enabled = false;
    }

    private void RestartValues()
    {
        meshRenderer.material.color = originalColor;

        stovePosition = null;
        playerDishPosition = null;

        rb.isKinematic = false;
        boxCollider.enabled = true;

        currentCookingState = CookingStates.Raw;

        cookTimeCounter = 0f;
        isInPlayerDishPosition = false;
        isServedInTable = false;
    }

    private void SaveTable(Table table)
    {
        if (isInPlayerDishPosition)
        {
            currentTable = table;
        }
    }

    private void ClearTable()
    {
        currentTable = null;
    }

    private void CheckCookingState()
    {
        if (cookTimeCounter < foodData.TimeToBeenCooked)
        {
            currentCookingState = CookingStates.Raw;
        }

        else if (cookTimeCounter >= foodData.TimeToBeenCooked && cookTimeCounter <= foodData.TimeToBeenCooked + foodData.TimeToBeenBurned)
        {
            currentCookingState = CookingStates.Cooked;
        }

        else if (cookTimeCounter > foodData.TimeToBeenCooked + foodData.TimeToBeenBurned)
        {
            currentCookingState = CookingStates.Burned;
        }
    }

    private void HandOver()
    {
        if (isInPlayerDishPosition && currentTable != null && currentTable.IsOccupied)
        {
            cookingManager.ReleaseDishPosition(playerDishPosition);

            Transform freeSpot = null;

            foreach (Transform spot in currentTable.DishPositions)
            {
                if (spot.childCount == 0)
                {
                    freeSpot = spot;
                    break;
                }
            }

            if (freeSpot != null)
            {
                transform.SetParent(freeSpot);
                transform.position = freeSpot.position + new Vector3(0, 0.1f, 0);

                rb.isKinematic = false;
                boxCollider.enabled = true;
                isInPlayerDishPosition = false;

                currentTable.CurrentFoods.Add(this); 

                ClearTable();
            }
        }
    }

    private void ThrowFoodToTrash()
    {
        if (gameObject.activeSelf && isInPlayerDishPosition)
        {
            cookingManager.ReleaseDishPosition(playerDishPosition);
            ReturnObjetToPool();
        }
    }
}
