using UnityEngine;

public enum ItemType { Currency, Weapon, Armor, Recipe, Misc }

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Dungeon/Create new Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    [TextArea] public string description;
    public ItemType type;
    public Sprite storeImage;
    [Header("3D Model")]
    public GameObject prefab; 
    [Header("Stats opcionales")]
    public int valueInTeeth; 
}
