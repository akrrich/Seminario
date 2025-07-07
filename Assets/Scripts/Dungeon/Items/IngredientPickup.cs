
using UnityEngine;

public class IngredientPickup : MonoBehaviour,IInteractable
{
    [SerializeField] private IngredientType ingredient;
    [SerializeField] private int amount = 1;
    [SerializeField] private bool destroyOnPickup = true;

    public void Interact()
    {
        IngredientInventoryManager.Instance.IncreaseIngredientStock(ingredient, amount);

        Debug.Log($"+{amount} {ingredient}");
        if (destroyOnPickup) Destroy(gameObject);
    }
}
