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
    private Transform positionWhenItsCooked;

    [SerializeField] private float timeToBeenCooked;
    [SerializeField] private FoodType foodType;


    void Awake()
    {
        GetComponents();
    }

    void OnEnable()
    {
        StartCoroutine(CookGameObject());
    }

    void Update()
    {
        // Provisorio para devolver el objeto al pool
        if (Input.GetKeyDown(KeyCode.Q))
        {
            cookingManager.ReleaseCookedPosition(positionWhenItsCooked);

            cookingManager.ReturnObjectToPool(foodType, this);
        }
    }


    private void GetComponents()
    {
        cookingManager = FindFirstObjectByType<CookingManager>();
    }
     
    private IEnumerator CookGameObject()
    {
        stovePosition = cookingManager.CurrentStove;

        yield return new WaitForSeconds(timeToBeenCooked);

        cookingManager.ReleaseStovePosition(stovePosition);

        positionWhenItsCooked = cookingManager.MoveFoodWhenIsCooked(this);
    }
}
