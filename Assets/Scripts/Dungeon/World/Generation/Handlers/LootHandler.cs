using UnityEngine;

public class LootHandler : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] barrelSpawnPoints;
    [SerializeField] private Transform[] chestSpawnPoints;

    [Header("Prefabs")]
    [SerializeField] private GameObject barrelPrefab;
    [SerializeField] private GameObject chestPrefab;

    [Header("Layer Modifiers")]
    [SerializeField] private float lootMultiplierPerLayer = 0.1f; // 10% más loot por capa

    public void SpawnLoot(RoomSize roomSize, int currentLayer = 1)
    {
        SpawnChest(roomSize, currentLayer);
        SpawnBarrels(roomSize, currentLayer);
    }

    private void SpawnChest(RoomSize roomSize, int currentLayer)
    {
        float baseChance = roomSize switch
        {
            RoomSize.Small => 0.20f,
            RoomSize.Medium => 0.30f,
            RoomSize.Large => 0.40f,
            _ => 0f
        };

        // Aumentar chance según capa
        float layerBonus = Mathf.Min(currentLayer * lootMultiplierPerLayer, 0.3f);
        float finalChance = baseChance + layerBonus;

        if (UnityEngine.Random.value < finalChance && chestSpawnPoints.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, chestSpawnPoints.Length);
            Instantiate(chestPrefab, chestSpawnPoints[index].position, Quaternion.identity, transform);
        }
    }

    private void SpawnBarrels(RoomSize roomSize, int currentLayer)
    {
        int baseBarrels = roomSize switch
        {
            RoomSize.Small => 1,
            RoomSize.Medium => 2,
            RoomSize.Large => 3,
            _ => 0
        };

        // Añadir barriles extras según capa
        int extraBarrels = Mathf.Min(currentLayer / 2, 3); // Máximo 3 extras
        int totalBarrels = baseBarrels + extraBarrels;

        // Asegurar que no excedamos los puntos de spawn
        totalBarrels = Mathf.Min(totalBarrels, barrelSpawnPoints.Length);

        for (int i = 0; i < totalBarrels; i++)
        {
            Instantiate(barrelPrefab, barrelSpawnPoints[i].position, Quaternion.identity, transform);
        }
    }

    public void Cleanup()
    {
        // Destruir todos los objetos de loot hijos
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Loot"))
            {
                Destroy(child.gameObject);
            }
        }
    }
}
