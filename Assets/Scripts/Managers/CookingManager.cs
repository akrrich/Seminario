using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingManager : MonoBehaviour
{
    [SerializeField] private GameObject rootGameObject; // GameObject padre con los botones hijos

    [SerializeField] private AbstractFactory abstractFactory;
    [SerializeField] private List<ObjectPooler> objectsPools;
    [SerializeField] private List<Button> buttonsFoods;

    [SerializeField] private List<Transform> stovesPositions;
    private Transform currentStove;
    private Queue<Transform> availableStovesPositions = new Queue<Transform>();
    private HashSet<Transform> occupiedStovesPositions = new HashSet<Transform>();

    [SerializeField] private List<Transform> cookedPositions;
    private Queue<Transform> availableCookedPositions = new Queue<Transform>(); 
    private HashSet<Transform> occupiedCookedPositions = new HashSet<Transform>();

    private Dictionary<FoodType, ObjectPooler> foodPoolDictionary = new Dictionary<FoodType, ObjectPooler>();

    public Transform CurrentStove { get => currentStove; }


    void Awake()
    {
        SuscribeToPlayerViewEvents();
        EnqueueStovesPositions();
        EnqueueCookedPosition();
        InitializeFoodPoolDictionary();
    }

    void OnDestroy()
    {
        UnSuscribeToPlayerViewEvents();
    }


    // Funcion asignada a los botones de la UI
    public void ButtonGetFood(int poolerIndex)
    {
        currentStove = GetNextAvailableStove();

        if (currentStove != null)
        {
            abstractFactory.CreateObject(currentStove, new Vector3(0, 0.2f, 0), objectsPools[poolerIndex]);
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


    private void SuscribeToPlayerViewEvents()
    {
        PlayerView.OnEnterInCookMode += () => ActiveOrDeactivateRootGameObject(true);
        PlayerView.OnExitInCookMode += () => ActiveOrDeactivateRootGameObject(false);
    }

    private void UnSuscribeToPlayerViewEvents()
    {
        PlayerView.OnEnterInCookMode -= () => ActiveOrDeactivateRootGameObject(true);
        PlayerView.OnExitInCookMode -= () => ActiveOrDeactivateRootGameObject(false);
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
