using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    public string enemyId;
    [Range(0, 100)]
    public float spawnChance; // % relativo dentro del conjunto
}

