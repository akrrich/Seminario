using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private List<EnemySpawner> enemySpawners = new();

    private readonly List<EnemyBase> activeEnemies = new();

    // ---------- PROPERTIES ----------
    public int EnemyCount => activeEnemies.Count;

    // ---------- EVENTS ----------
    public event Action OnAllEnemiesDefeated;

    // ---------- PUBLIC API ----------

    /// <summary>
    /// Inicializa el handler y activa un número aleatorio de spawners.
    /// </summary>
    public void Initialize(RoomConfig config, int layer)
    {
        Cleanup();

        if (enemySpawners.Count == 0)
        {
            Debug.LogWarning($"[EnemyHandler] Sala {name} no tiene spawners configurados.");
            return;
        }

        // Determinar cuántos spawners se activan
        int minSpawns = config.baseEnemyCount;
        int maxSpawns = Mathf.Min(config.enemySpawnPointCount, enemySpawners.Count);

        int spawnCount = UnityEngine.Random.Range(minSpawns, maxSpawns + 1);

        // Elegir aleatoriamente los spawners a activar
        List<EnemySpawner> shuffled = new List<EnemySpawner>(enemySpawners);
        shuffled = RouletteSelection.Shuffle(shuffled);

        for (int i = 0; i < spawnCount; i++)
        {
            EnemySpawner spawner = shuffled[i];
            List<EnemyBase> spawned = spawner.SpawnEnemies(layer);

            foreach (var enemy in spawned)
            {
                RegisterEnemy(enemy);
            }
        }

        Debug.Log($"[EnemyHandler] Spawneados {activeEnemies.Count} enemigos en sala {name}.");
    }

    /// <summary>
    /// Limpia enemigos activos y resetea spawners.
    /// </summary>
    public void Cleanup()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
            {
                enemy.OnDeath -= HandleEnemyDeath;
                Destroy(enemy.gameObject);
            }
        }
        activeEnemies.Clear();

        foreach (var spawner in enemySpawners)
            spawner.ResetSpawner();
    }

    // ---------- PRIVATE METHODS ----------

    private void RegisterEnemy(EnemyBase enemy)
    {
        if (enemy == null) return;

        activeEnemies.Add(enemy);
        enemy.OnDeath += HandleEnemyDeath;
    }

    private void HandleEnemyDeath(EnemyBase deadEnemy)
    {
        deadEnemy.OnDeath -= HandleEnemyDeath;
        activeEnemies.Remove(deadEnemy);

        Debug.Log($"[EnemyHandler] Enemigo derrotado. Restantes: {activeEnemies.Count}");

        if (activeEnemies.Count == 0)
        {
            Debug.Log($"[EnemyHandler] Todos los enemigos derrotados en sala {name}.");
            OnAllEnemiesDefeated?.Invoke();
        }
    }
}
