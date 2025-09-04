using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] private SpawnerData spawnerData;
    private Dictionary<string, EnemyBase> enemiesDictionary;

    private void Awake()
    {
        enemiesDictionary = new Dictionary<string, EnemyBase>();
        foreach (EnemyBase enemy in spawnerData.enemies)
        {
            if (enemy == null || string.IsNullOrEmpty(enemy.Id)) continue;
            if (!enemiesDictionary.ContainsKey(enemy.Id))
                enemiesDictionary.Add(enemy.Id, enemy);
            else
                Debug.LogWarning($"[EnemyFactory] Id duplicado: {enemy.Id}");
        }
    }

    public EnemyBase Create(string id, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        if (enemiesDictionary != null && enemiesDictionary.TryGetValue(id, out EnemyBase enemy))
        {
            return Instantiate(enemy, spawnPosition, spawnRotation);
        }
        Debug.LogWarning($"[EnemyFactory] No se encontró prefab con Id '{id}'");
        return null;
    }

    public EnemyBase GetPrefab(string id)
    {
        if (enemiesDictionary != null && enemiesDictionary.TryGetValue(id, out EnemyBase enemy))
            return enemy;
        return null;
    }

}
