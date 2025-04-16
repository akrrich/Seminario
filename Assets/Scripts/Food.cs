using System.Collections;
using UnityEngine;

public enum FoodType
{
    Mouse, Fish,
}

public class Food : MonoBehaviour
{
    private CookingManager cookingManager;
    private Table currentTable;

    private Transform stovePosition;
    private Transform cookedPosition;
    private Transform dishPosition;

    private Rigidbody rb;
    private BoxCollider boxCollider;

    [SerializeField] private float timeToBeenCooked;
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

    void OnDisable()
    {
        RestartValues();
    }

    void OnDestroy()
    {
        UnsuscribeToPlayerControllerEvents();
    }


    public void ReturnObjetToPool()
    {
        cookingManager.ReturnObjectToPool(foodType, this);
    }


    private void SuscribeToPlayerControllerEvents()
    {
        PlayerController.OnGrabFood += Grab;
        PlayerController.OnHandOverFood += HandOver;

        PlayerController.OnTableCollisionEnter += SaveTable;
        PlayerController.OnTableCollisionExit += ClearTable;
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
    {
        if (isCooked && !isInPlayerDishPosition)
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
            cookingManager.ReleaseDishPosition(dishPosition);

            transform.SetParent(currentTable.DishPosition);
            transform.position = currentTable.DishPosition.position + new Vector3(0, 0.1f, 0);

            rb.isKinematic = false;
            boxCollider.enabled = true;
            isInPlayerDishPosition = false;
            isCooked = false;

            currentTable.CurrentFood = this;

            ClearTable();
        }
    }
}
