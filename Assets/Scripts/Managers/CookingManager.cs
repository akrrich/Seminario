using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingManager : MonoBehaviour
{
    // Recordartorio: Desactivar los botones si no hay mas comida disponible en ese item

    [SerializeField] private GameObject rootGameObject; // GameObject padre con los botones hijos

    [SerializeField] private AbstractFactory foodAbstractFactory;
    [SerializeField] private List<ObjectPooler> objectsPools;
    [SerializeField] private List<Button> buttonsFoods;

    // Para las posiciones de las sarten
    [SerializeField] private List<Transform> stovesPositions;
    private Transform currentStove;
    private Queue<Transform> availableStovesPositions = new Queue<Transform>();
    private HashSet<Transform> occupiedStovesPositions = new HashSet<Transform>();

    // Para las posiciones cuando se terminan de cocinar
    [SerializeField] private List<Transform> cookedPositions;
    private Queue<Transform> availableCookedPositions = new Queue<Transform>(); 
    private HashSet<Transform> occupiedCookedPositions = new HashSet<Transform>();

    // Para las posiciones en la bandeja del player
    [SerializeField] private List<Transform> dishPositions;
    private Queue<Transform> availableDishPositions = new Queue<Transform>();
    private HashSet<Transform> occupiedDishPositions = new HashSet<Transform>();

    private Dictionary<FoodType, ObjectPooler> foodPoolDictionary = new Dictionary<FoodType, ObjectPooler>();

    private Action onEnterCook, onExitCook;

    public Transform CurrentStove { get => currentStove; }
    public Queue<Transform> AvailableDishPositions { get => availableDishPositions; }


    void Awake()
    {
        InitializeLambdaEvents();
        SuscribeToPlayerViewEvents();
        EnqueueStovesPositions();
        EnqueueCookedPosition();
        EnqueueDishPositions();
        InitializeFoodPoolDictionary();
    }

    void OnDestroy()
    {
        UnSuscribeToPlayerViewEvents();
    }


    // Funcion asignada a los botones de la UI
    public void ButtonGetFood(string prefabFoodName)
    {
        FoodType foodType = (FoodType)Enum.Parse(typeof(FoodType), prefabFoodName);

        if (InventoryFoodManager.Instance.GetFoodStock(foodType) > 0)
        {
            InventoryFoodManager.Instance.ConsumeFood(foodType);

            currentStove = GetNextAvailableStove();

            if (currentStove != null)
            {
                foodAbstractFactory.CreateObject(prefabFoodName, currentStove, new Vector3(0, 0.2f, 0));
            }
        }
    }

    public void ReturnObjectToPool(FoodType foodType, Food currentFood)
    {
        if (foodPoolDictionary.ContainsKey(foodType))
        {
            foodPoolDictionary[foodType].ReturnObjectToPool(currentFood);
        }
    }

    public void ReleaseStovePosition(Transform stovePosition)
    {
        // Solamanete liberar posiciones de sarten si hay lugar en los espacios donde va la comida cocinada
        if (availableCookedPositions.Count > 0)
        {
            if (occupiedStovesPositions.Contains(stovePosition))
            {
                occupiedStovesPositions.Remove(stovePosition);
                availableStovesPositions.Enqueue(stovePosition);
            }
        }
    }

    public Transform MoveFoodWhenIsCooked(Food currentFood)
    {
        Transform targetPosition = null;

        while (availableCookedPositions.Count > 0)
        {
            targetPosition = availableCookedPositions.Dequeue();

            if (occupiedCookedPositions.Contains(targetPosition))
            {
                availableCookedPositions.Enqueue(targetPosition);  
                continue;
            }

            occupiedCookedPositions.Add(targetPosition);
            break;
        }

        if (targetPosition != null)
        {
            currentFood.transform.position = targetPosition.position;
        }

        return targetPosition;
    }

    public void ReleaseCookedPosition(Transform cookedPosition)
    {
        if (occupiedCookedPositions.Contains(cookedPosition))
        {
            occupiedCookedPositions.Remove(cookedPosition); 
            availableCookedPositions.Enqueue(cookedPosition);
        }
    }

    public Transform MoveFoodToDish(Food currentFood)
    {
        Transform targetPosition = null;

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
            currentFood.transform.position = targetPosition.position;
            currentFood.transform.SetParent(targetPosition);
        }

        return targetPosition;
    }

    public void ReleaseDishPosition(Transform dishPosition)
    {
        if (occupiedDishPositions.Contains(dishPosition))
        {
            occupiedDishPositions.Remove(dishPosition);
            availableDishPositions.Enqueue(dishPosition);
        }
    }

    private Transform GetNextAvailableStove()
    {
        Transform targetPosition = null;

        if (availableStovesPositions.Count == 0)
        {
            return null;
        }

        while (availableStovesPositions.Count > 0)
        {
            targetPosition = availableStovesPositions.Dequeue();

            if (occupiedStovesPositions.Contains(targetPosition))
            {
                availableStovesPositions.Enqueue(targetPosition);
                continue;
            }

            occupiedStovesPositions.Add(targetPosition);
            break;
        }

        return targetPosition;
    }

    private void InitializeLambdaEvents()
    {
        onEnterCook += () => ActiveOrDeactivateRootGameObject(true);
        onExitCook += () => ActiveOrDeactivateRootGameObject(false);
    }

    private void SuscribeToPlayerViewEvents()
    {
        PlayerView.OnEnterInCookMode += onEnterCook;
        PlayerView.OnExitInCookMode += onExitCook;
    }

    private void UnSuscribeToPlayerViewEvents()
    {
        PlayerView.OnEnterInCookMode -= onEnterCook;
        PlayerView.OnExitInCookMode -= onExitCook;
    }

    private void ActiveOrDeactivateRootGameObject(bool state)
    {
        rootGameObject.SetActive(state);
    }

    private void EnqueueStovesPositions()
    {
        foreach (var position in stovesPositions)
        {
            availableStovesPositions.Enqueue(position);
        }
    }

    private void EnqueueCookedPosition()
    {
        foreach (var position in cookedPositions)
        {
            availableCookedPositions.Enqueue(position);
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
        for (int i = 0; i < objectsPools.Count; i++)
        {
            if (Enum.IsDefined(typeof(FoodType), i)) 
            {
                FoodType foodType = (FoodType) i;
                foodPoolDictionary[foodType] = objectsPools[i]; 
            }
        }
    }
}
