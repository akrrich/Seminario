using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] EnemyFactory enemyFactory;
    [SerializeField] private Transform spawnPosition;

    [Header("Spawns Data and % ")]
    [SerializeField] private EnemySpawnTableData enemySpawnTable;
    [SerializeField] private int layer; //capa de la dungeon
    [SerializeField] private StatScaler statScaler;

    [Header("Spawn Variables")]
    [SerializeField] private SpawnerConfigData spawnerConfigData;

    private int spawnedEnemies = 0;
    /// <summary>
    /// Spawnea enemigos de acuerdo a la tabla de probabilidades y la configuración.
    /// </summary>
    public List<EnemyBase> SpawnEnemies(int layer)
    {
        List<EnemyBase> result = new();

        int spawnCount = Random.Range(
            spawnerConfigData.MinEnemiesPerSpawn,
            spawnerConfigData.MaxEnemiesPerSpawn + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            string selectedId = GetEnemyIdFromTable();
            EnemyBase enemy = enemyFactory.Create(
                selectedId,
                spawnPosition.position,
                spawnPosition.rotation);

            if (enemy != null)
            {
                statScaler?.ApplyScaling(enemy, layer);
                result.Add(enemy);
                spawnedEnemies++;

                if (spawnedEnemies >= spawnerConfigData.MaxSpawnedEnemies)
                {
                    Debug.Log("[EnemySpawner] Alcanzado máximo de enemigos.");
                    break;
                }
            }
            else
            {
                Debug.LogWarning($"[EnemySpawner] No se pudo crear enemigo con ID {selectedId}");
            }
        }

        return result;
    }

    private string GetEnemyIdFromTable()
    {
        Dictionary<string, float> weightedDict = new();
        foreach (var entry in enemySpawnTable.spawnDataList)
        {
            weightedDict.Add(entry.enemyId, entry.spawnChance);
        }
        return RouletteSelection.Roulette(weightedDict);
    }

}
