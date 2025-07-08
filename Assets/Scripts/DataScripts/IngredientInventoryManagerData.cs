using UnityEngine;

[CreateAssetMenu(fileName = "IngredientInventoryManagerData", menuName = "ScriptableObjects/Create New IngredientInventoryManagerData")]
public class IngredientInventoryManagerData : ScriptableObject
{
    [SerializeField] private int initializeStockForAllIngredients;
    [SerializeField] private bool ingredientsUseOwnStock;

    public int InitializeStockForAllIngredients { get => initializeStockForAllIngredients; }
    public bool IngredientsUseOwnStock { get => ingredientsUseOwnStock; }
}
