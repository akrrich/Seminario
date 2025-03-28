using System.Collections;
using UnityEngine;

public enum FoodType
{
    Mouse,
    Fish,
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

    void Update()
    {
        // Provisorio para agarrar las comidas
        if (Input.GetKeyDown(KeyCode.Q) && isCooked && !isInPlayerDishPosition)
        {
            cookingManager.ReleaseCookedPosition(cookedPosition);
            dishPosition = cookingManager.MoveFoodToDish(this);

            StartCoroutine(DisablePhysics());

            isInPlayerDishPosition = true;
        }

        // Provisorio para "entregar" las comidas
        if (Input.GetKeyDown(KeyCode.Z) && isInPlayerDishPosition)
        {
            cookingManager.ReleaseDishPosition(dishPosition);
            cookingManager.ReturnObjectToPool(foodType, this);
        }
    }


    private void GetComponents()
    {
        cookingManager = FindFirstObjectByType<CookingManager>();
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }
     
    private IEnumerator CookGameObject()
    {
        stovePosition = cookingManager.CurrentStove;

        yield return new WaitForSeconds(timeToBeenCooked);

        cookingManager.ReleaseStovePosition(stovePosition);
        cookedPosition = cookingManager.MoveFoodWhenIsCooked(this);

        isCooked = true;
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
}
