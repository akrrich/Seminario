using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatModifier
{
    public int layer; // N�mero de capa
    [Header("Multipliers")]

    [Range(0, 1f)]
    public float hpMultiplier = 0f;

    [Range(0, 1f)]
    public float damageMultiplier = 0f;

    [Range(0, 1f)]
    public float speedMultiplier = 0f;
}
