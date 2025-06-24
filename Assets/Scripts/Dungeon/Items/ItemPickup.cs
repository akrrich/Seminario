
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private bool destroyOnPickup = true;

    public void Interact()          // tu interfaz es void Interact()
    {
        switch (itemData.type)
        {
            // ————————————————— Currency (dientes)
            case ItemType.Currency:
                MoneyManager.Instance.AddMoney(itemData.valueInTeeth);
                Debug.Log($"+{itemData.valueInTeeth} dientes");
                break;

            // ————————————————— Weapon
            case ItemType.Weapon:
                // TODO: implementa PlayerEquipment más adelante
                Debug.Log($"Arma obtenida: {itemData.itemName} (daño {itemData.damage})");
                break;

            // ————————————————— Armor
            case ItemType.Armor:
                // TODO: PlayerEquipment.ObtainArmor()
                Debug.Log($"Armadura obtenida: {itemData.itemName} (defensa {itemData.defense})");
                break;

            // ————————————————— Recipe
            case ItemType.Recipe:
                // TODO: RecipeBook.Instance.Unlock(itemData);
                Debug.Log($"Receta desbloqueada: {itemData.itemName}");
                break;

            // ————————————————— Otros
            case ItemType.Misc:
                Debug.Log($"Objeto misceláneo obtenido: {itemData.itemName}");
                break;
        }

        if (destroyOnPickup) Destroy(gameObject);
    }
}