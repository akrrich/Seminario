using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] EnemyFactory enemyFactory;
    [SerializeField] private GameObject spawnPosition;

    [Header("Spawns Data and % ")]
    [SerializeField] private EnemySpawnTableData enemySpawnTable;
    [SerializeField] private int layer; //capa de la dungeon
    [SerializeField] private StatScaler statScaler;

    [Header("Spawn Variables")]
    [SerializeField] private SpawnerConfigData spawnerConfigData;

    private int spawnedEnemies = 0;
    private float lastSpawnTime = -Mathf.Infinity;
    private bool isActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (!other.CompareTag("Player")) return;

        if (Time.time - lastSpawnTime >= spawnerConfigData.SpawnCooldown && spawnedEnemies < spawnerConfigData.MaxSpawnedEnemies)
        {
            SpawnEnemy();
        }
        else
        {
            Debug.Log("Cooldown activo o se alcanzó el límite de enemigos.");
        }
    }

    private void Update()
    {
        // Si el spawner está inactivo, contamos el tiempo para reactivarlo
        if (!isActive && Time.time - lastSpawnTime >= spawnerConfigData.SpawnerResetTime)
        {
            isActive = true;
            spawnedEnemies = 0;
            Debug.Log("Spawner reactivado. Enemigos generados reiniciados.");
        }
    }

    private void SpawnEnemy()
    {
        string selectedId = GetEnemyIdFromTable();
        EnemyBase enemy = enemyFactory.Create(selectedId, spawnPosition.transform.position, spawnPosition.transform.rotation);

        if (enemy != null && statScaler != null)
        {
            statScaler.ApplyScaling(enemy, layer);
            spawnedEnemies++;
            lastSpawnTime = Time.time;
            Debug.Log($"Spawner activo. Enemigos generados: {spawnedEnemies}/{spawnerConfigData.MaxSpawnedEnemies}");

            if (spawnedEnemies >= spawnerConfigData.MaxSpawnedEnemies)
            {
                isActive = false;
                Debug.Log("Spawner desactivado. Esperando reactivación...");
            }
        }
        else
        {
            Debug.LogWarning("No se pudo instanciar el enemigo.");
        }
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
