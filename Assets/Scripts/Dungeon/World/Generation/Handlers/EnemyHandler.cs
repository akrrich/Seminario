using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    public event Action OnAllEnemiesDefeated;

    private List<GameObject> activeEnemies = new();
    public int EnemyCount => activeEnemies.Count;

    public void Initialize()
    {
        Cleanup();
    }

    public void SpawnEnemies(RoomConfig config, int layer)
    {
        int spawnCount = config.GetEnemyCountForLayer(layer);
        int availableSpawns = Mathf.Min(spawnCount, config.enemySpawnPointCount);

        for (int i = 0; i < availableSpawns; i++)
        {
            var prefab = config.possibleEnemies[UnityEngine.Random.Range(0, config.possibleEnemies.Length)];
            var spawnPoint = GetRandomSpawnPoint();

            if (prefab != null && spawnPoint != null)
            {
                GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity, transform);
                var enemyBase = enemy.GetComponent<EnemyBase>();
                if (enemyBase != null)
                {
                    enemyBase.OnDeath += HandleEnemyDeath;
                }
                activeEnemies.Add(enemy);
            }
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        // Implementá una forma de obtener puntos de spawn reales si querés algo más preciso
        return transform;
    }

    private void HandleEnemyDeath(EnemyBase deadEnemy)
    {
        deadEnemy.OnDeath -= HandleEnemyDeath;
        activeEnemies.Remove(deadEnemy.gameObject);

        if (activeEnemies.Count == 0)
        {
            OnAllEnemiesDefeated?.Invoke();
        }
    }

    public void Cleanup()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        activeEnemies.Clear();
    }
}
