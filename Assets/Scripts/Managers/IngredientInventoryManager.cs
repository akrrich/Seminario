using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientInventoryManager : MonoBehaviour
{
    // Lo que hay que hacer es sincronizar los botones de compra de ingredientes cuando se desbloquean 

    private static IngredientInventoryManager instance;

    private PlayerModel playerModel;

    [Header("Ingredientes")]
    [SerializeField] private List<IngredientType> availableIngredients;
    [SerializeField] private List<Ingredients.FoodRecipe> foodRecipes;
    [SerializeField] private List<Ingredients.IngredientData> ingredientData;

    [Header("UI")]
    [SerializeField] private List<GameObject> startSlotPrefabs;
    private RawImage inventoryPanel;
    private Transform slotParentObject;
    private List<Transform> slotPositions = new List<Transform>();

    private Dictionary<IngredientType, int> ingredientInventory = new();
    private Dictionary<IngredientType, (GameObject slot, TextMeshProUGUI text)> ingredientSlots = new();
    private Dictionary<FoodType, Ingredients.FoodRecipe> recipeDict = new();
    private Dictionary<IngredientType, Ingredients.IngredientData> ingredientDataDict = new();

    [Header("")]
    [SerializeField] private int initializeStockIngredients;

    private bool isInventoryOpenUI = false;

    public static IngredientInventoryManager Instance => instance;


    void Awake()
    {
        InitializeSingleton();
        GetComponents();
        SubscribeToPlayerViewEvent();
        InitializeInventory();
        InitializeIngredientData();
        InitializeRecipes();
    }

    void Update()
    {
        EnabledOrDisabledInventoryPanel();

        // Test
        if (Input.GetKeyDown(KeyCode.T))
        {
            UnlockNewIngredient(IngredientType.Water, "WaterSlot", 1);
        }
    }

    void OnDestroy()
    {
        UnsubscribeToPlayerViewEvent();
    }


    public bool HasUnlockIngredient(IngredientType ingredient)
    {
        return ingredientInventory.ContainsKey(ingredient);
    }

    public void IncreaseIngredientStock(IngredientType ingredient)
    {
        ingredientInventory[ingredient]++;
    }

    public bool TryCraftFood(FoodType foodType)
    {
        if (!recipeDict.ContainsKey(foodType))
        {
            return false;
        }

        var recipe = recipeDict[foodType];

        foreach (var ing in recipe.Ingridients)
        {
            if (!ingredientInventory.ContainsKey(ing.IngredientType) || ingredientInventory[ing.IngredientType] < ing.Amount)
            {
                return false;
            }
        }

        foreach (var ing in recipe.Ingridients)
        {
            ingredientInventory[ing.IngredientType] -= ing.Amount;
        }

        return true;
    }

    public int GetPriceOfIngredient(IngredientType ingredient)
    {
        return ingredientDataDict.TryGetValue(ingredient, out var data) ? data.Price : 0;
    }

    public void UnlockNewIngredient(IngredientType newIngredient, string prefabName, int stockForIngredient)
    {
        if (!ingredientInventory.ContainsKey(newIngredient))
        {
            availableIngredients.Add(newIngredient);
            ingredientInventory[newIngredient] = stockForIngredient;

            GameObject slotPrefab = Resources.Load<GameObject>("Prefabs/InventorySlot/" + prefabName); 

            if (slotPrefab != null)
            {
                int nextSlotIndex = availableIngredients.Count - 1;

                if (nextSlotIndex < slotPositions.Count)
                {
                    GameObject slotInstance = Instantiate(slotPrefab, slotPositions[nextSlotIndex].position, Quaternion.identity, slotParentObject);
                    slotInstance.SetActive(false);

                    TextMeshProUGUI stockText = slotInstance.GetComponentInChildren<TextMeshProUGUI>();
                    ingredientSlots[newIngredient] = (slotInstance, stockText);
                }
            }
        }
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

        slotParentObject = GameObject.Find("SlotObjects").transform;
        inventoryPanel = GameObject.Find("InventoryPanel").GetComponent<RawImage>();    

        Transform slotParentPositions = GameObject.Find("SlotTransforms").transform;
        foreach (Transform slotChilds in slotParentPositions)
        {
            slotPositions.Add(slotChilds.GetComponent<Transform>());
        }
    }

    private void SubscribeToPlayerViewEvent()
    {
        PlayerView.OnDeactivateInventoryFoodUI += HideInventory;
    }

    private void UnsubscribeToPlayerViewEvent()
    {
        PlayerView.OnDeactivateInventoryFoodUI -= HideInventory;
    }

    private void InitializeInventory()
    {
        foreach (IngredientType ingredient in availableIngredients)
        {
            ingredientInventory[ingredient] = initializeStockIngredients;
        }

        for (int i = 0; i < startSlotPrefabs.Count; i++)
        {
            GameObject slotInstance = Instantiate(startSlotPrefabs[i], slotPositions[i].position, Quaternion.identity, slotParentObject);
            slotInstance.SetActive(false);

            IngredientType type = availableIngredients[i];
            TextMeshProUGUI stockText = slotInstance.GetComponentInChildren<TextMeshProUGUI>();

            ingredientSlots[type] = (slotInstance, stockText);
        }
    }

    private void InitializeRecipes()
    {
        recipeDict = foodRecipes.ToDictionary(r => r.FoodType, r => r);
    }

    private void InitializeIngredientData()
    {
        foreach (var data in ingredientData)
        {
            if (!ingredientDataDict.ContainsKey(data.IngredientType))
            {
                ingredientDataDict[data.IngredientType] = data;
            }
        }
    }

    private void ShowInventory()
    {
        isInventoryOpenUI = true;
        inventoryPanel.enabled = true;

        foreach (var kvp in ingredientSlots)
        {
            kvp.Value.slot.SetActive(true);
            kvp.Value.text.text = ingredientInventory[kvp.Key].ToString();
        }
    }

    private void HideInventory()
    {
        isInventoryOpenUI = false;
        inventoryPanel.enabled = false;

        foreach (var kvp in ingredientSlots)
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