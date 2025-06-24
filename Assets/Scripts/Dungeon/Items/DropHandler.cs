using System;
using UnityEngine;

[RequireComponent(typeof(DropTable))]
public class DropHandler : MonoBehaviour
{
    [SerializeField] private DropTable table;
    [SerializeField] private LootPrefabDatabase lootDB;
    [SerializeField] private Transform spawnPoint;   // dónde cae el loot


    private void Awake() => table = GetComponent<DropTable>();
    public void Init(DropTable t, LootPrefabDatabase db, Transform point)
    {
        table = t;
        lootDB = db;
        spawnPoint = point != null ? point : transform;
    }
    public void DropLoot()
    {
        foreach (var entry in table.Roll())  
            Spawn(entry);
    }

    public void Spawn(DropEntry e)
    {
        GameObject prefab = lootDB.GetPrefab(e.lootName);
        if (prefab == null)
        {
            Debug.LogWarning($"Sin prefab para «{e.lootName}»");
            return;
        }

        GameObject go = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // — si es ingrediente, inyectamos tipo + cantidad —
        if (go.TryGetComponent(out IngredientPickup ing))
        {
            if (System.Enum.TryParse(e.lootName, out IngredientType type))
                ing.GetType().GetField("ingredient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                             ?.SetValue(ing, type);

            int qty = UnityEngine.Random.Range(e.minAmount, e.maxAmount + 1);
            ing.GetType().GetField("amount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                         ?.SetValue(ing, qty);
        }
    }
}