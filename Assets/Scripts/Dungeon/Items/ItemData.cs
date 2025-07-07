using UnityEngine;

public enum ItemType { Currency, Weapon, Armor, Recipe, Misc }

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Dungeon/Create new Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    [TextArea] public string description;
    public ItemType type;
    public Sprite icon;

    [Header("Stats opcionales")]
    public int valueInTeeth;  // para monedas o precio de venta
}
