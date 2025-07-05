using System.Collections;
using UnityEngine;

public enum FoodType
{
    VegetableSoup, BeefStew, TomatoAndLettuceSalad, BeastMeatPie, HumanFleshStew
}

public enum CookingState
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

    private Transform stovePosition; // Posicion de las hornallas
    private Transform dishPosition; // Posicion del plato del player

    private Rigidbody rb;
    private BoxCollider boxCollider;
    private MeshRenderer meshRenderer;
    private Color originalColor;


    [SerializeField] private FoodType foodType;
    private CookingState currentCookingState = CookingState.Raw;

    private float cookTimeCounter = 0f;

    private bool isInstantiateFirstTime = true;
    private bool isInPlayerDishPosition = false;


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

        PlayerController.OnTableCollisionEnterForHandOverFood += SaveTable;
        PlayerController.OnTableCollisionExitForHandOverFood += ClearTable;
    }

    private void UnsuscribeToPlayerControllerEvents()
    {
        PlayerController.OnGrabFood -= Grab;
        PlayerController.OnHandOverFood -= HandOver;

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

    private void Initialize()
    {
        originalColor = meshRenderer.material.color;
        currentCookingState = CookingState.Raw;
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
        dishPosition = null;

        rb.isKinematic = false;
        boxCollider.enabled = true;

        currentCookingState = CookingState.Raw;

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
        float cookedTime = foodData.TimeToBeenCooked;

        if (cookTimeCounter < cookedTime)
        {
            Debug.Log("Crudo");
            currentCookingState = CookingState.Raw;
        }

        else if (cookTimeCounter <= cookedTime + foodData.TimeToBeenBurned)
        {
            Debug.Log("Cocinado");
            currentCookingState = CookingState.Cooked;
        }

        else
        {
            Debug.Log("Quemado");
            currentCookingState = CookingState.Burned;
        }
    }

    /// <summary>
    /// Solucionar el problema de que agarra todas las comidas que estan en las hornallas, unicamente tiene que agarrar la que esta mirando
    /// </summary>
    private void Grab()
    {   // Es importante que este activo para que no haya errores cuando se invoca el evento      // Si hay posiciones disponibles en la bandeja
        if (gameObject.activeSelf && !isInPlayerDishPosition && cookingManager.AvailableDishPositions.Count > 0)
        {
            cookingManager.ReleaseStovePosition(stovePosition);
            dishPosition = cookingManager.MoveFoodToDish(this);

            StartCoroutine(DisablePhysics());

            isInPlayerDishPosition = true;
        }
    }

    private void HandOver()
    {
        if (isInPlayerDishPosition && currentTable != null && currentTable.IsOccupied)
        {
            PlayerView.OnHandOverCompletedForHandOverMessage?.Invoke();

            cookingManager.ReleaseDishPosition(dishPosition);

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
}
