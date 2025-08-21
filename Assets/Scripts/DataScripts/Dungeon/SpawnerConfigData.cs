using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerInfo", menuName = "ScriptableObjects/Dungeon/Create New SpawnerInfo")]
public class SpawnerConfigData : ScriptableObject
{
    [SerializeField] private float spawnCooldown = 5f; 
    [SerializeField] private int maxSpawnedEnemies = 10;
    [SerializeField] private float spawnerResetTime = 60f;

    public float SpawnCooldown { get => spawnCooldown; }
    public int MaxSpawnedEnemies { get => maxSpawnedEnemies; }
    public float SpawnerResetTime { get => spawnerResetTime; }


}
