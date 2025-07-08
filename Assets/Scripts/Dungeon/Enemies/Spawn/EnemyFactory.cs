using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] private SpawnerData spawnerData;
    Dictionary<string, EnemyBase> enemiesDictionary;

    private void Awake()
    {
        enemiesDictionary = new Dictionary<string, EnemyBase>();
        
        foreach (EnemyBase enemy in spawnerData.enemies)
        {
            enemiesDictionary.Add(enemy.Id, enemy);
        }
    }
    public EnemyBase Create(string id, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        if (enemiesDictionary.TryGetValue(id, out EnemyBase enemy))
        {
            return Instantiate(enemy, spawnPosition, spawnRotation);
        }
        return null;
    }

}
