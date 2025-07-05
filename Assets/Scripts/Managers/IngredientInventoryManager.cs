using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientInventoryManager : Singleton<IngredientInventoryManager>
{
    private List<IngredientType> availableIngredients;
    [SerializeField] private List<Ingredients.FoodRecipe> foodRecipes;
    [SerializeField] private List<Ingredients.IngredientData> ingredientData;
    [SerializeField] private int initializeStockIngredients;

    private Dictionary<IngredientType, int> ingredientInventory = new();
    private Dictionary<FoodType, Ingredients.FoodRecipe> recipeDict = new();
    private Dictionary<IngredientType, Ingredients.IngredientData> ingredientDataDict = new();
    
    public Dictionary<IngredientType, Ingredients.IngredientData> IngredientDataDict { get =>  ingredientDataDict; }


    void Awake()
    {
        CreateSingleton(true);
        InitializeInventory();
        InitializeIngredientData();
        InitializeRecipes();
    }


    public void IncreaseIngredientStock(IngredientType ingredient)
    {
        ingredientInventory[ingredient]++;
    }

    public bool TryCraftFood(FoodType foodType)
    {
        if (!recipeDict.ContainsKey(foodType)) return false;

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

    public int GetStock(IngredientType ingredient)
    {
        return ingredientInventory.TryGetValue(ingredient, out var stock) ? stock : 0;
    }

    public Ingredients.FoodRecipe GetRecipe(FoodType foodType)
    {
        return recipeDict.TryGetValue(foodType, out var recipe) ? recipe : null;
    }

    public List<IngredientType> GetAllIngredients() => new List<IngredientType>(ingredientInventory.Keys);


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