using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryFoodManager : MonoBehaviour
{
    private static InventoryFoodManager instance;

    private PlayerModel playerModel;

    [SerializeField] private List<Food> foodPrefabs = new List<Food>();

    private Dictionary<FoodType, int> foodInventory = new Dictionary<FoodType, int>();

    private Image inventoryPanel;

    [SerializeField] private int initializeStockForFoods;

    private bool isInventoryOpenUI = false;

    public static InventoryFoodManager Instance { get => instance; }


    void Awake()
    {
        InitializeSingleton();
        SuscribeToPlayerViewEvent();
        InitializeInventory();
        GetComponents();
    }

    void Update()
    {
        EnabledOrDisabledInventoryPanel();
    }

    void OnDestroy()
    {
        UnsuscribeToPlayerViewEvent();
    }


    public void BuyFood(FoodType foodType)
    {
        foodInventory[foodType]++;
    }

    public bool ConsumeFood(FoodType foodType)
    {
        if (foodInventory[foodType] > 0)
        {
            foodInventory[foodType]--;
            return true;
        }

        return false;
    }

    public int GetFoodStock(FoodType foodType)
    {
        return foodInventory.ContainsKey(foodType) ? foodInventory[foodType] : 0;
    }

    public int GetPriceOfFood(FoodType type)
    {
        return foodPrefabs.FirstOrDefault(f => f.FoodType == type)?.Price ?? 0;
    }


    private void InitializeSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void SuscribeToPlayerViewEvent()
    {
        PlayerView.OnDeactivateInventoryFoodUi += DeactivateInventoryUIForced;
    }

    private void UnsuscribeToPlayerViewEvent()
    {
        PlayerView.OnDeactivateInventoryFoodUi -= DeactivateInventoryUIForced;
    }

    private void InitializeInventory()
    {
        foreach (FoodType food in Enum.GetValues(typeof(FoodType)))
        {
            foodInventory[food] = initializeStockForFoods;
        }
    }

    private void GetComponents()
    {
        playerModel = FindFirstObjectByType<PlayerModel>();
        inventoryPanel = GameObject.Find("InventoryPanel").GetComponent<Image>();
    }

    private void EnabledOrDisabledInventoryPanel()
    {
        if (!playerModel.IsCooking && !playerModel.IsAdministrating)
        {
            if (Input.GetKeyDown(KeyCode.Tab) && !isInventoryOpenUI)
            {
                isInventoryOpenUI = true;
                inventoryPanel.enabled = true;
            }

            else if (Input.GetKeyDown(KeyCode.Tab) && isInventoryOpenUI)
            {
                isInventoryOpenUI = false;
                inventoryPanel.enabled = false;
            }
        }
    }

    private void DeactivateInventoryUIForced()
    {
        isInventoryOpenUI = false;
        inventoryPanel.enabled = false;
    }
}
