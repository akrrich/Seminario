using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatScaler : MonoBehaviour 
{
    [SerializeField]
    public List<StatModifierData> modifiers = new();

    public void ApplyScaling(EnemyBase enemy, int currentLayer)
    {
        StatModifierData mod = modifiers.Find(m => m.layer == currentLayer);
        if (mod == null)
        {
            Debug.LogWarning($"[StatScaler] No hay modificador definido para la capa {currentLayer}.");
            return;
        }
        enemy.SetScaledStats(mod.hpMultiplier, mod.damageMultiplier, mod.speedMultiplier);
    }
}
