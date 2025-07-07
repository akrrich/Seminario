using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] EnemyFactory enemyFactory;
    [SerializeField] private GameObject spawnPosition;

    [Header("Configuración de Spawns")]
    [SerializeField] private EnemySpawnTable enemySpawnTable;
    [SerializeField] private int layer; //capa de la dungeon
    [SerializeField] private StatScaler statScaler; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            string selectedId = GetEnemyIdFromTable();
            EnemyBase enemy = enemyFactory.Create(selectedId, spawnPosition.transform.position, spawnPosition.transform.rotation);
            if (enemy != null && statScaler != null)
            {
                statScaler.ApplyScaling(enemy, layer);
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
