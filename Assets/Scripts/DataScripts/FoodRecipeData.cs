using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FoodRecipeData", menuName = "ScriptableObjects/Tabern/Create New FoodRecipeData")]
public class FoodRecipeData : ScriptableObject
{
    [SerializeField] private FoodType foodType;
    [SerializeField] private List<IngredientAmount> ingridients;

    public FoodType FoodType { get => foodType; }
    public List<IngredientAmount> Ingridients { get => ingridients; }
    

    [Serializable]
    public class IngredientAmount // Para decirle la cantidad de ingredientes correspondiente por comida
    {
        [SerializeField] private IngredientType ingredientType;
        [SerializeField] private int amount;

        public IngredientType IngredientType { get => ingredientType; }
        public int Amount { get => amount; }
    }
}
