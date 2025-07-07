using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(DropHandler))]
public class Barrel : MonoBehaviour, IDamageable
{
    [Header("Properties")]
    [SerializeField] private int hitPoints = 1;

    [Header("Loot System")]
    [SerializeField] private DropTable dropTable;
    [SerializeField] private LootPrefabDatabase lootDB;
    [SerializeField] private Transform lootSpawnPoint;

    private DropHandler dropHandler;

    public void TakeDamage(int value)
    {
        TakeHit(value);
    }

    public void TakeHit(int value)
    {
        hitPoints -= value;
        if (hitPoints <= 0)
        {
            BreakBarrel();
        }
    }

    private void BreakBarrel()
    {
        // Generamos el loot usando el DropHandler
        dropHandler.DropLoot();

        Destroy(gameObject);
    }
}
