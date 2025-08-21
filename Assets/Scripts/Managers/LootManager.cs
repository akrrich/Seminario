using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance { get; private set; }

    [SerializeField] private LootPrefabDatabase lootDatabase;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnLoot(string lootName, Vector3 position)
    {
        GameObject prefab = lootDatabase.GetPrefab(lootName);
        if (prefab != null)
        {
            Instantiate(prefab, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"[LootManager] Loot '{lootName}' no registrado en la base.");
        }
    }
}
