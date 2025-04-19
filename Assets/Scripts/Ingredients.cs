using System;
using System.Collections.Generic;
using UnityEngine;

public enum IngredientType
{
    Rat,
    Salmon,
    Water,
}

public class Ingredients
{
    [Serializable]
    public class FoodRecipe
    {
        [SerializeField] private FoodType foodType;
        [SerializeField] private List<IngredientAmount> ingridients;

        public FoodType FoodType { get => foodType; }
        public List<IngredientAmount> Ingridients { get => ingridients; }
    }

    [Serializable]
    public class IngredientAmount
    {
        [SerializeField] private IngredientType ingredientType;
        [SerializeField] private int amount;

        public IngredientType IngredientType { get => ingredientType; }
        public int Amount { get => amount; }
    }

    [Serializable]
    public class IngredientData
    {
        [SerializeField] private IngredientType ingridientType;
        [SerializeField] private int price;

        public IngredientType IngredientType { get => ingridientType; }
        public int Price { get => price; }
    }
}
