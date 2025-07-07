
using UnityEngine;

public class DropHandler : MonoBehaviour
{
    [SerializeField] private DropTable table;
    [SerializeField] private LootPrefabDatabase lootDB;
    [SerializeField] private Transform spawnPoint;   // dónde cae el loot


    private void Awake()
    {
        if (spawnPoint == null)
            spawnPoint = transform;
    }
    public void Init(DropTable t, LootPrefabDatabase db, Transform point)
    {
        table = t;
        lootDB = db;
        spawnPoint = point != null ? point : transform;
    }
    public void DropLoot()
    {
        if (table == null || lootDB == null)
        {
            Debug.LogWarning($"DropHandler en {name}: Falta asignar DropTable o LootDB.");
            return;
        }
        DropEntry entry = table.Roll();
        if (entry != null)
        {
            Spawn(entry);
        }
        else
        {
            Debug.LogWarning($"No se obtuvo loot de la tabla.");
        }
    }

    private void Spawn(DropEntry e)
    {
        GameObject prefab = lootDB.GetPrefab(e.lootName);
        if (prefab == null)
        {
            Debug.LogWarning($"Sin prefab para «{e.lootName}»");
            return;
        }

        GameObject go = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // Inyección de datos si es ingrediente
        if (go.TryGetComponent(out IngredientPickup ing))
        {
            if (System.Enum.TryParse(e.lootName, out IngredientType type))
            {
                ing.GetType().GetField("ingredient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(ing, type);
            }

            int qty = UnityEngine.Random.Range(e.minAmount, e.maxAmount + 1);
            ing.GetType().GetField("amount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ing, qty);
        }
    }
}