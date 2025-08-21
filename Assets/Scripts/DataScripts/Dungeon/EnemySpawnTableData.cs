using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnTableData", menuName = "ScriptableObjects/Dungeon/Create New EnemySpawnTableData")]
public class EnemySpawnTableData : ScriptableObject
{
    public List<EnemySpawnData> spawnDataList = new();
}
