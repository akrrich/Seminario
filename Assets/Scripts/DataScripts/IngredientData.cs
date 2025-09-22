using UnityEngine;

[CreateAssetMenu(fileName = "IngredientData", menuName = "ScriptableObjects/Create New IngredientData")]
public class IngredientData : ScriptableObject
{
    [SerializeField] private IngredientType ingridientType;
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject prefabSlotInventoryUI;
    [SerializeField] private int price;
    [SerializeField] private int initializeStock;

    public IngredientType IngredientType { get => ingridientType; }
    public Sprite Sprite { get => sprite; }
    public GameObject PrefabSlotInventoryUI { get => prefabSlotInventoryUI; }
    public int Price { get => price; }
    public int InitializeStock { get => initializeStock; }
}
