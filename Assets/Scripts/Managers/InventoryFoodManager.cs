using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryFoodManager : MonoBehaviour
{
    private static InventoryFoodManager instance;

    private PlayerModel playerModel;

    [SerializeField] private List<Food> foodPrefabs; // Prefabs de las comidas
    [SerializeField] private List<GameObject> slotPrefabs; // Prefabs de los slots
    [SerializeField] private List<Transform> slotPositions; // Posiciones donde se instancian los slots
    [SerializeField] private Transform slotParent; // Transform que representa donde se instancian los slots

    private Dictionary<FoodType, int> foodInventory = new Dictionary<FoodType, int>();
    private Dictionary<FoodType, (GameObject slot, TextMeshProUGUI text)> foodSlots = new();

    private RawImage inventoryPanel;

    [SerializeField] private int initializeStockForFoods;

    private bool isInventoryOpenUI = false;                             

    public static InventoryFoodManager Instance { get => instance; }


    void Awake()
    {
        InitializeSingleton();
        GetComponents();
        SuscribeToPlayerViewEvent();
        InitializeInventory();
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

    private void GetComponents()
    {
        playerModel = FindFirstObjectByType<PlayerModel>();
        inventoryPanel = GameObject.Find("InventoryPanel").GetComponent<RawImage>();
    }

    private void SuscribeToPlayerViewEvent()
    {
        PlayerView.OnDeactivateInventoryFoodUI += HideInventory;
    }

    private void UnsuscribeToPlayerViewEvent()
    {
        PlayerView.OnDeactivateInventoryFoodUI -= HideInventory;
    }

    private void InitializeInventory()
    {
        // Inicializar stock
        foreach (FoodType food in Enum.GetValues(typeof(FoodType)))
        {
            foodInventory[food] = initializeStockForFoods;
        }

        // Inicializar en la UI
        for (int i = 0; i < slotPrefabs.Count && i < slotPositions.Count && i < foodPrefabs.Count; i++)
        {
            GameObject slotInstance = Instantiate(slotPrefabs[i], slotPositions[i].position, Quaternion.identity, slotParent);
            slotInstance.SetActive(false);

            FoodType type = foodPrefabs[i].FoodType;
            TextMeshProUGUI stockText = slotInstance.GetComponentInChildren<TextMeshProUGUI>();

            foodSlots[type] = (slotInstance, stockText);
        }
    }

    private void ShowInventory()
    {
        isInventoryOpenUI = true;
        inventoryPanel.enabled = true;

        foreach (var kvp in foodSlots)
        {
            kvp.Value.slot.SetActive(true);
            kvp.Value.text.text = foodInventory[kvp.Key].ToString();
        }
    }

    private void HideInventory()
    {
        isInventoryOpenUI = false;
        inventoryPanel.enabled = false;

        foreach (var kvp in foodSlots)
        {
            kvp.Value.slot.SetActive(false);
        }
    }

    private void EnabledOrDisabledInventoryPanel()
    {
        if (!playerModel.IsCooking && !playerModel.IsAdministrating)
        {
            if (PlayerInputs.Instance.Inventory())
            {
                (isInventoryOpenUI ? (Action)HideInventory : ShowInventory)();
            }
        }
    }
}
