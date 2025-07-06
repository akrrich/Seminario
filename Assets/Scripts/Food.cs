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

public class Food : MonoBehaviour
{
    // No usar el metodo OnDisabled de Unity

    [SerializeField] private FoodData foodData;

    private CookingManager cookingManager;
    private Table currentTable; // Esta Table hace referencia a la mesa en la cual podemos entregar el pedido

    private Rigidbody rb;
    private BoxCollider boxCollider;
    private MeshRenderer meshRenderer;
    private GameObject foodTriggerGameObject;
    private Color originalColor;

    private Transform stovePosition;
    private Transform playerDishPosition;

    [SerializeField] private FoodType foodType;
    private CookingStates currentCookingState;

    private float cookTimeCounter = 0f;

    private bool isInstantiateFirstTime = true;
    private bool isInPlayerDishPosition = false;

    public CookingStates CurrentCookingState { get => currentCookingState; }


    void Awake()
    {
        SuscribeToPlayerControllerEvents();
        GetComponents();
        Initialize();
    }

    void OnEnable()
    {
        StartCoroutine(CookGameObject());
    }

    void OnDestroy()
    {
        UnsuscribeToPlayerControllerEvents();
    }


    public void ReturnObjetToPool()
    {
        cookingManager.ReturnObjectToPool(foodType, this);
        RestartValues();
    }


    private void SuscribeToPlayerControllerEvents()
    {
        PlayerController.OnGrabFood += Grab;
        PlayerController.OnHandOverFood += HandOver;
        PlayerController.OnThrowFoodToTrash += ThrowFoodToTrash;

        PlayerController.OnTableCollisionEnterForHandOverFood += SaveTable;
        PlayerController.OnTableCollisionExitForHandOverFood += ClearTable;
    }

    private void UnsuscribeToPlayerControllerEvents()
    {
        PlayerController.OnGrabFood -= Grab;
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
        foodTriggerGameObject = transform.Find("FoodTrigger").gameObject;
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
        foodTriggerGameObject.tag = "FoodTrigger";
        foodTriggerGameObject.layer = LayerMask.NameToLayer("FoodTrigger");
        meshRenderer.material.color = originalColor;

        stovePosition = null;
        playerDishPosition = null;

        rb.isKinematic = false;
        boxCollider.enabled = true;

        currentCookingState = CookingStates.Raw;

        cookTimeCounter = 0f;
        isInPlayerDishPosition = false;
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
            Debug.Log("Crudo");
            currentCookingState = CookingStates.Raw;
        }

        else if (cookTimeCounter >= foodData.TimeToBeenCooked && cookTimeCounter <= foodData.TimeToBeenCooked + foodData.TimeToBeenBurned)
        {
            Debug.Log("Cocinado");
            currentCookingState = CookingStates.Cooked;
        }

        else if (cookTimeCounter > foodData.TimeToBeenCooked + foodData.TimeToBeenBurned)
        {
            Debug.Log("Quemado");
            currentCookingState = CookingStates.Burned;
        }
    }

    private void Grab(Food currentFood)
    {
        if (currentFood == this)
        {
            // Es importante que este activo para que no haya errores cuando se invoca el evento      // Si hay posiciones disponibles en la bandeja
            if (gameObject.activeSelf && !isInPlayerDishPosition && cookingManager.AvailableDishPositions.Count > 0)
            {
                isInPlayerDishPosition = true;
                foodTriggerGameObject.tag = "Untagged";
                foodTriggerGameObject.layer = LayerMask.NameToLayer("Default");

                cookingManager.ReleaseStovePosition(stovePosition);
                playerDishPosition = cookingManager.MoveFoodToDish(this);

                StartCoroutine(DisablePhysics());
            }
        }
    }

    private void HandOver()
    {
        if (isInPlayerDishPosition && currentTable != null && currentTable.IsOccupied)
        {
            PlayerView.OnHandOverCompletedForHandOverMessage?.Invoke();

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
