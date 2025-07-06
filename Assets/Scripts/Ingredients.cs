using System;
using System.Collections.Generic;
using UnityEngine;

public enum IngredientType // Flour = Harina
{
    Onion, Carrot, Potato, BeastMeat, Tomato, Lettuce, Oil, Flour, Pepper, HumanFlesh
}

public class Ingredients
{
    [Serializable]
    public class FoodRecipe // Para machear la cantidad de ingredientes por comida
    {
        [SerializeField] private FoodType foodType;
        [SerializeField] private List<IngredientAmount> ingridients;

        public FoodType FoodType { get => foodType; }
        public List<IngredientAmount> Ingridients { get => ingridients; }
    }

    [Serializable]
    public class IngredientAmount // Para decirle la cantidad de ingredientes correspondiente por comida
    {
        [SerializeField] private IngredientType ingredientType;
        [SerializeField] private int amount;

        public IngredientType IngredientType { get => ingredientType; }
        public int Amount { get => amount; }
    }

    [Serializable]
    public class IngredientData // Para datos de los ingredientes si es necesario en un futuro agregar mas
    {
        [SerializeField] private IngredientType ingridientType;
        [SerializeField] private Sprite sprite;
        [SerializeField] private int price;

        public IngredientType IngredientType { get => ingridientType; }
        public Sprite Sprite { get => sprite; }
        public int Price { get => price; }
    }
}
