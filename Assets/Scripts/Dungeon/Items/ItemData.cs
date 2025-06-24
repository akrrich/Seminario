using UnityEngine;

public enum ItemType { Currency, Weapon, Armor, Recipe, Misc }

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    [TextArea] public string description;
    public ItemType type;
    public Sprite icon;

    [Header("Stats opcionales")]
    public int damage;        // armas
    public int defense;       // armaduras
    public int valueInTeeth;  // para monedas o precio de venta
}
