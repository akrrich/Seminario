using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RecipeInfoUI : MonoBehaviour
{
    [SerializeField] private List<SlotUI> slots; // Asignás 3 elementos en el inspector
    [SerializeField] private Sprite defaultSprite; // Sprite vacío por si faltan ingredientes
    [SerializeField] private List<IngredientSpriteData> ingredientSprites; // Para machear tipo con imagen

    private Dictionary<IngredientType, Sprite> ingredientSpriteDict = new();

    [System.Serializable]
    public class SlotUI
    {
        public Image ingredientImage;
        public TextMeshProUGUI amountText;
    }

    [System.Serializable]
    public class IngredientSpriteData
    {
        public IngredientType type;
        public Sprite sprite;
    }

    void Awake()
    {
        foreach (var data in ingredientSprites)
        {
            ingredientSpriteDict[data.type] = data.sprite;
        }
    }

    // Funcion asignada a la UI
    public void ShowRecipeIngredients(string foodTypeName)
    {
        if (!System.Enum.TryParse(foodTypeName, out FoodType foodType)) return;

        var recipe = IngredientInventoryManager.Instance.GetRecipe(foodType);
        if (recipe == null) return;

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < recipe.Ingridients.Count)
            {
                var ing = recipe.Ingridients[i];
                slots[i].amountText.text = ing.Amount.ToString();
                slots[i].ingredientImage.sprite = ingredientSpriteDict.TryGetValue(ing.IngredientType, out var sprite) ? sprite : defaultSprite;
            }
            else
            {
                slots[i].amountText.text = "";
                slots[i].ingredientImage.sprite = defaultSprite;
            }
        }
    }
}
