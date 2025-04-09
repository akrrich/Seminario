using System.Collections;
using UnityEngine;

public enum FoodType
{
    Mouse, Fish,
}

public class Food : MonoBehaviour
{
    private CookingManager cookingManager;

    private Transform stovePosition;
    private Transform cookedPosition;
    private Transform dishPosition;

    private Rigidbody rb;
    private BoxCollider boxCollider;

    [SerializeField] private float timeToBeenCooked;
    [SerializeField] private FoodType foodType;

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


    private void SuscribeToPlayerControllerEvents()
    {
        PlayerController.OnGrabFood += Grab;
        PlayerController.OnHandOverFood += HandOver;
    }

    private void UnsuscribeToPlayerControllerEvents()
    {
        PlayerController.OnGrabFood -= Grab;
        PlayerController.OnHandOverFood -= HandOver;
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
        if (!isInPlayerDishPosition)
        {
            stovePosition = cookingManager.CurrentStove;

            yield return new WaitForSeconds(timeToBeenCooked);

            cookingManager.ReleaseStovePosition(stovePosition);
            cookedPosition = cookingManager.MoveFoodWhenIsCooked(this);

            isCooked = true;
        }
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
        if (isInPlayerDishPosition)
        {
            cookingManager.ReleaseDishPosition(dishPosition);
            cookingManager.ReturnObjectToPool(foodType, this);

            RestartValues();
        }
    }
}
