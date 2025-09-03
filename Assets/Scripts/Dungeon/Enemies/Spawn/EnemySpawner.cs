using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyFactory enemyFactory;
    [SerializeField] private EnemySpawnTableData enemySpawnTable;
    [SerializeField] private StatScaler statScaler;

    private bool hasSpawned = false;

    public List<EnemyBase> SpawnEnemies(int layer)
    {
        if (hasSpawned)
            return new List<EnemyBase>();

        List<EnemyBase> result = new();

        // Acá podés ajustar según config de sala (min/max enemigos por spawner)
        int spawnCount = Random.Range(1, 3);

        for (int i = 0; i < spawnCount; i++)
        {
            string selectedId = GetEnemyIdFromTable();
            EnemyBase enemy = enemyFactory.Create(
                selectedId,
                transform.position,
                transform.rotation);

            if (enemy != null)
            {
                statScaler?.ApplyScaling(enemy, layer);
                result.Add(enemy);
            }
        }

        hasSpawned = true;
        return result;
    }

    public void ResetSpawner()
    {
        hasSpawned = false;
    }

    private string GetEnemyIdFromTable()
    {
        Dictionary<string, float> weightedDict = new();
        foreach (var entry in enemySpawnTable.spawnDataList)
            weightedDict.Add(entry.enemyId, entry.spawnChance);

        return RouletteSelection.Roulette(weightedDict);
    }

}
