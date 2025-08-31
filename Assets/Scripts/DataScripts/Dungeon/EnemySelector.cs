using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/Enemy Spawn Config")]
public class EnemySpawnConfig : ScriptableObject
{
    [System.Serializable]
    public class LayerEnemySet
    {
        [Tooltip("Capa mínima (inclusive) en la que este grupo de enemigos puede aparecer.")]
        public int minLayer;

        [Tooltip("Capa máxima (inclusive) en la que este grupo de enemigos puede aparecer.")]
        public int maxLayer;

        [Tooltip("Lista de enemigos posibles para este rango de capas. Cada enemigo tiene un peso que representa su probabilidad relativa.")]
        public List<WeightedEnemy> enemies;
    }

    [System.Serializable]
    public class WeightedEnemy
    {
        [Tooltip("Prefab del enemigo que puede spawnearse en este rango de capas. Debe tener un componente que herede de EnemyBase.")]
        public GameObject enemyPrefab;

        [Tooltip("Peso relativo para el sistema de ruleta. Cuanto mayor el valor, más chances tiene este enemigo de ser elegido.")]
        public float weight;
    }

    [Tooltip("Configuraciones de enemigos por rango de capas. Se evaluarán de arriba hacia abajo y se usará la primera que coincida con la capa actual.")]
    [SerializeField] private List<LayerEnemySet> layerConfigs;

    public System.Type GetEnemyTypeForLayer(int layer)
    {
        foreach (var config in layerConfigs)
        {
            if (layer >= config.minLayer && layer <= config.maxLayer)
            {
                Dictionary<System.Type, float> options = new();

                foreach (var w in config.enemies)
                {
                    if (w.enemyPrefab.TryGetComponent(out EnemyBase enemyBase))
                    {
                        options[enemyBase.GetType()] = w.weight;
                    }
                }

                return RouletteSelection.Roulette(options);
            }
        }

        return null;
    }
}
