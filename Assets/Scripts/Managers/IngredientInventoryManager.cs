using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientInventoryManager : MonoBehaviour
{
    private static IngredientInventoryManager instance;

    private List<IngredientType> availableIngredients;
    [SerializeField] private List<Ingredients.FoodRecipe> foodRecipes;
    [SerializeField] private List<Ingredients.IngredientData> ingredientData;
    [SerializeField] private int initializeStockIngredients;

    private Dictionary<IngredientType, int> ingredientInventory = new();
    private Dictionary<FoodType, Ingredients.FoodRecipe> recipeDict = new();
    private Dictionary<IngredientType, Ingredients.IngredientData> ingredientDataDict = new();
    
    public static IngredientInventoryManager Instance { get => instance; }


    void Awake()
    {
        CreateSingleton();
        InitializeInventory();
        InitializeIngredientData();
        InitializeRecipes();
    }


    public void IncreaseIngredientStock(IngredientType ingredient, int amount)
    {
        if (!ingredientInventory.ContainsKey(ingredient))
            ingredientInventory[ingredient] = 0;

        ingredientInventory[ingredient] += amount;

    }
    public void IncreaseIngredientStock(IngredientType ingredient)
    {
        IncreaseIngredientStock(ingredient, 1);
    }

    public bool TryCraftFood(FoodType foodType)
    {
        if (!recipeDict.ContainsKey(foodType)) return false;

        var recipe = recipeDict[foodType];

        foreach (var ing in recipe.Ingredients)
        {
            if (!ingredientInventory.ContainsKey(ing.IngredientType) || ingredientInventory[ing.IngredientType] < ing.Amount)
            {
                return false;
            }
        }

        foreach (var ing in recipe.Ingredients)
        {
            ingredientInventory[ing.IngredientType] -= ing.Amount;
        }

        return true;
    }

    public int GetPriceOfIngredient(IngredientType ingredient)
    {
        return ingredientDataDict.TryGetValue(ingredient, out var data) ? data.Price : 0;
    }

    public int GetStock(IngredientType ingredient)
    {
        return ingredientInventory.TryGetValue(ingredient, out var stock) ? stock : 0;
    }

    public List<IngredientType> GetAllIngredients() => new List<IngredientType>(ingredientInventory.Keys);


    private void CreateSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void InitializeInventory()
    {
        availableIngredients = Enum.GetValues(typeof(IngredientType)).Cast<IngredientType>().ToList();

        foreach (IngredientType ingredient in availableIngredients)
        {
            ingredientInventory[ingredient] = initializeStockIngredients;
        }
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

    private void InitializeRecipes()
    {
        recipeDict = foodRecipes.ToDictionary(r => r.FoodType, r => r);
    }
}