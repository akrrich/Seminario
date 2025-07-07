using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] EnemyFactory enemyFactory;
    [SerializeField] private GameObject spawnPosition;

    [Header("Configuración de Spawns")]
    [SerializeField] private EnemySpawnTable enemySpawnTable;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            string selectedId = GetEnemyIdFromTable();
            if (!string.IsNullOrEmpty(selectedId))
            {
                enemyFactory.Create(selectedId, spawnPosition.transform.position, spawnPosition.transform.rotation);
            }
            else
            {
                Debug.LogWarning("No enemy selected by roulette.");
            }
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
