using System.Collections;
using UnityEngine;

public enum FoodType
{
    VegetableSoup, BeefStew, TomatoAndLettuceSalad, BeastMeatPie, HumanFleshStew
}

public class Food : MonoBehaviour
{
    // No usar el metodo OnDisabled de Unity

    [SerializeField] private FoodData foodData;

    private CookingManager cookingManager;
    private Table currentTable; // Esta Table hace referencia a la mesa en la cual podemos entregar el pedido

    private Transform stovePosition;
    private Transform cookedPosition;
    private Transform dishPosition;

    private Rigidbody rb;
    private BoxCollider boxCollider;

    [SerializeField] private FoodType foodType;

    private bool isInstantiateFirstTime = true;
    private bool isCooked = false;
    private bool isInPlayerDishPosition = false;


    void Awake()
    {
        SuscribeToPlayerControllerEvents();
        GetComponents();
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
    }

    // Ejecutar unicamente la corrutina cuando se activa el objeto, si se activo porque entrego la comida
    private IEnumerator CookGameObject()
    {
        if (!isInPlayerDishPosition && !isInstantiateFirstTime)
        {
            stovePosition = cookingManager.CurrentStove;

            yield return new WaitForSeconds(foodData.TimeToBeenCooked);

            cookingManager.ReleaseStovePosition(stovePosition);
            cookedPosition = cookingManager.MoveFoodWhenIsCooked(this);

            isCooked = true;
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
        stovePosition = null;
        cookedPosition = null;
        dishPosition = null;

        rb.isKinematic = false;
        boxCollider.enabled = true;

        isCooked = false;
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

    private void Grab()
    {                                               // Si hay posiciones disponibles en la bandeja
        if (isCooked && !isInPlayerDishPosition && cookingManager.AvailableDishPositions.Count > 0)
        {
            cookingManager.ReleaseCookedPosition(cookedPosition);
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
                isCooked = false;

                currentTable.CurrentFoods.Add(this); 

                ClearTable();
            }
        }
    }
}
