using System;
using System.Collections.Generic;
using UnityEngine;

public class CookingManager : Singleton<CookingManager>
{
    [SerializeField] private AbstractFactory foodAbstractFactory;
    [SerializeField] private List<ObjectPooler> foodPools;

    private PlayerView playerView;
    private CookingDeskUI currentDesk;
    private Transform currentStove;

    private event Action<int> onAvailableStoveIndex;

    // Para las posiciones en la bandeja del player
    [SerializeField] private List<Transform> dishPositions;
    private Queue<Transform> availableDishPositions = new Queue<Transform>();
    private HashSet<Transform> occupiedDishPositions = new HashSet<Transform>();

    private Dictionary<FoodType, ObjectPooler> foodPoolDictionary = new Dictionary<FoodType, ObjectPooler>();

    public CookingDeskUI CurrentDesk => currentDesk;

    public Transform CurrentStove { get => currentStove; }
    public List<Transform> DishPositions { get => dishPositions; }
    public Queue<Transform> AvailableDishPositions { get => availableDishPositions; }
    public HashSet<Transform> OccupiedDishPositions {  get => occupiedDishPositions; }

    public Action<int> OnAvailableStoveIndex { get => onAvailableStoveIndex; set => onAvailableStoveIndex = value; }


    void Awake()
    {
        CreateSingleton(false);
        SuscribeToCookingManagerUIEvent();
        EnqueueDishPositions();
        InitializeFoodPoolDictionary();
        playerView = FindAnyObjectByType<PlayerView>();
    }

    void OnDestroy()
    {
        UnsuscribeToCookingManagerUIEvent();
    }


    public void SetCurrentDesk(CookingDeskUI desk)
    {
        currentDesk = desk;
    }

    public void ReturnObjectToPool(FoodType foodType, Food currentFood)
    {
        if (foodPoolDictionary.ContainsKey(foodType))
        {
            foodPoolDictionary[foodType].ReturnObjectToPool(currentFood);
        }
    }

    public void ReturnAllObjectsToPool()
    {
        Food[] allFoods = FindObjectsByType<Food>(FindObjectsSortMode.None);

        foreach (Food food in allFoods)
        {
            if (food.gameObject.activeSelf)
            {
                if (foodPoolDictionary.TryGetValue(food.FoodType, out ObjectPooler pool))
                {
                    pool.ReturnObjectToPool(food);
                }
            }
        }
    }

    public bool HasActiveDishes()
    {
        foreach (Transform dishPosition in playerView.Dish.transform)
        {
            if (dishPosition.childCount > 0)
            {
                return true;
            }
        }

        // 2?? Verificar si hay alguna Food activa en la escena
        Food[] allFoods = FindObjectsByType<Food>(FindObjectsSortMode.None);

        foreach (Food food in allFoods)
        {
            if (food.gameObject.activeSelf)
            {
                return true;
            }
        }

        // 3?? No hay nada en ningún lado
        return false;
    }

    public void ReleaseStovePosition(Transform stovePosition)
    {
        currentDesk?.ReleaseStove(stovePosition);
    }

    public void ReleaseDishPosition(Transform dishPosition)
    {
        if (occupiedDishPositions.Contains(dishPosition))
        {
            occupiedDishPositions.Remove(dishPosition);
            availableDishPositions.Enqueue(dishPosition);
        }
    }

    public Transform MoveFoodToDish(Food currentFood)
    {
        /*Transform targetPosition = null;

        while (availableDishPositions.Count > 0)
        {
            targetPosition = availableDishPositions.Dequeue();

            if (occupiedDishPositions.Contains(targetPosition))
            {
                availableDishPositions.Enqueue(targetPosition);
                continue;
            }

            occupiedDishPositions.Add(targetPosition);
            break;
        }

        if (targetPosition != null)
        {
            currentFood.transform.SetParent(targetPosition);
            float offsetY = currentFood.GetBottomOffset() - 0.030f;
            currentFood.transform.rotation = targetPosition.rotation;
            currentFood.transform.position = targetPosition.position + new Vector3(0, offsetY, 0);
        }

        return targetPosition;*/

        for (int i = 0; i < dishPositions.Count; i++)
        {
            Transform targetPosition = dishPositions[i];

            if (!occupiedDishPositions.Contains(targetPosition))
            {
                occupiedDishPositions.Add(targetPosition);

                currentFood.transform.SetParent(targetPosition);
                float offsetY = currentFood.GetBottomOffset() - 0.030f;
                currentFood.transform.rotation = targetPosition.rotation;
                currentFood.transform.position = targetPosition.position + new Vector3(0, offsetY, 0);

                return targetPosition;
            }
        }

        return null;
    }


    private void SuscribeToCookingManagerUIEvent()
    {
        CookingManagerUI.OnButtonSetFood += GetFood;
    }

    private void UnsuscribeToCookingManagerUIEvent()
    {
        CookingManagerUI.OnButtonSetFood -= GetFood;
    }

    private void GetFood(string prefabFoodName, bool ClickWellOrClickWrong)
    {
        if (ClickWellOrClickWrong)
        {
            if (currentDesk.AvailableStoves.Count == 0)
            {
                AudioManager.Instance.PlayOneShotSFX("ButtonClickWrong");
                return;
            }

            AudioManager.Instance.PlayOneShotSFX("ButtonClickWell");
        }

        else
        {
            AudioManager.Instance.PlayOneShotSFX("ButtonClickWrong");
            return;
        }

        if (Enum.TryParse(prefabFoodName, out FoodType foodType))
        {
            if (IngredientInventoryManager.Instance.TryCraftFood(foodType))
            {
                currentStove = currentDesk?.GetAvailableStove();

                if (currentStove != null)
                {
                    foodAbstractFactory.CreateObject(prefabFoodName, currentStove, new Vector3(0, 0.2f, 0));
                    int index = currentDesk.StoveIndexOf(currentStove);
                    onAvailableStoveIndex?.Invoke(index);
                }
            }
        }
    }

    private void EnqueueDishPositions()
    {
        foreach (var position in dishPositions)
        {
            availableDishPositions.Enqueue(position);
        }
    }

    private void InitializeFoodPoolDictionary()
    {
        for (int i = 0; i < foodPools.Count; i++)
        {
            GameObject prefab = foodPools[i].Prefab;

            if (Enum.TryParse(prefab.name, out FoodType foodType))
            {
                foodPoolDictionary[foodType] = foodPools[i];
            }
        }
    }
}
