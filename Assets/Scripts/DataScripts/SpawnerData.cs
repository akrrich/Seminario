using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerData", menuName = "ScriptableObjects/Dungeon/Create New SpawnerData")]
public class SpawnerData : ScriptableObject
{
    [SerializeField] public EnemyBase[] enemies;
}
