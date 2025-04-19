using System.Collections;
using UnityEngine;

public enum FoodType
{
    Mouse, Fish
}

public class Food : MonoBehaviour
{
    // No usar el metodo OnDisabled de Unity

    private CookingManager cookingManager;
    private Table currentTable;

    private Transform stovePosition;
    private Transform cookedPosition;
    private Transform dishPosition;

    private Rigidbody rb;
    private BoxCollider boxCollider;

    [SerializeField] private FoodType foodType;
    [SerializeField] private float timeToBeenCooked;

    private bool isInstantiateFirstTime = true;
    private bool isCooked = false;
    private bool isInPlayerDishPosition = false;
    private bool isClientInChair = false;


    void Awake()
    {
        SuscribeToPlayerControllerEvents();
        SuscribeToClientModelEvent();
        GetComponents();
    }

    void OnEnable()
    {
        StartCoroutine(CookGameObject());
    }

    void OnDestroy()
    {
        UnsuscribeToPlayerControllerEvents();
        UnsuscribeToClientModelEvent();
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

        PlayerController.OnTableCollisionEnter += SaveTable;
        PlayerController.OnTableCollisionExit += ClearTable;
    }

    private void SuscribeToClientModelEvent()
    {
        ClientModel.OnWaitingFood += SaveClientInChair;
    }

    private void UnsuscribeToClientModelEvent()
    {
        ClientModel.OnWaitingFood -= SaveClientInChair;
    }

    private void UnsuscribeToPlayerControllerEvents()
    {
        PlayerController.OnGrabFood -= Grab;
        PlayerController.OnHandOverFood -= HandOver;

        PlayerController.OnTableCollisionEnter -= SaveTable;
        PlayerController.OnTableCollisionExit -= ClearTable;
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

            yield return new WaitForSeconds(timeToBeenCooked);

            cookingManager.ReleaseStovePosition(stovePosition);
            cookedPosition = cookingManager.MoveFoodWhenIsCooked(this);

            isCooked = true;
        }

        isInstantiateFirstTime = false;
    }

    private IEnumerator DisablePhysics()
    {
        yield return new WaitForSeconds(0.1f);
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
        isClientInChair = false;
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

    private void SaveClientInChair()
    {
        isClientInChair = true;
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
        if (isInPlayerDishPosition && currentTable != null && currentTable.IsOccupied && isClientInChair)
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
