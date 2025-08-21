using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootDatabase", menuName = "ScriptableObjects/Loot Prefab Database")]
public class LootPrefabDatabase : ScriptableObject
{
    [System.Serializable]
    public class LootEntry
    {
        public string lootName;
        public GameObject prefab;
    }

    [SerializeField] private List<LootEntry> lootEntries;

    private Dictionary<string, GameObject> prefabLookup;

    public GameObject GetPrefab(string lootName)
    {
        if (prefabLookup == null)
        {
            prefabLookup = new Dictionary<string, GameObject>();
            foreach (var entry in lootEntries)
            {
                if (!prefabLookup.ContainsKey(entry.lootName))
                {
                    prefabLookup.Add(entry.lootName, entry.prefab);
                }
            }
        }

        prefabLookup.TryGetValue(lootName, out GameObject result);
        return result;
    }
}
