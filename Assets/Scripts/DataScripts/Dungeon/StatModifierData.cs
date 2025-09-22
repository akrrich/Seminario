using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatModifierData", menuName = "ScriptableObjects/Dungeon/Create New EnemyStatModifierData")]

public class StatModifierData : ScriptableObject
{
    public int layer; // Número de capa
    [Header("Multipliers")]

    [Range(0, 1f)]
    public float hpMultiplier = 0f;

    [Range(0, 1f)]
    public float damageMultiplier = 0f;

    [Range(0, 1f)]
    public float speedMultiplier = 0f;
}
