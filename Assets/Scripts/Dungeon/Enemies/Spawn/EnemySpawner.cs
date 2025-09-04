using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Tooltip("Colocalo en la sala. Se encarga de spawnear enemigos en puntos definidos. Usado por EnemyHandler.")]
public class EnemySpawner : MonoBehaviour
{
    [Header("Refs")]
    [Tooltip("Factory de enemigos que instancia prefabs en base al Id. (Obligatorio)")]
    [SerializeField] private EnemyFactory enemyFactory;

    [Tooltip("Scaler de estadísticas. Aplica modificadores según la capa actual. (Opcional, pero recomendado)")]
    [SerializeField] private StatScaler statScaler;

    [Header("Spawnpoints")]
    [Tooltip("Lista de Transforms que definen las posiciones posibles de spawn. El sistema elige uno al azar cada vez.")]
    [SerializeField] private List<Transform> spawnPoints = new();

    /*
    [Header("Markers")]
    [SerializeField] private GameObject spawnMarkerPrefab;
    [SerializeField] private float preSpawnDelay = 0.75f;
    */

    private readonly Dictionary<string, Queue<EnemyBase>> pools = new();

    // ---------- API PRINCIPAL ----------
    /// <summary>
    /// Spawnea 'amount' enemigos en esta estación, usando puntos random.
    /// Llama onSpawned por cada enemigo creado/activado.
    /// </summary>
    public IEnumerator SpawnEnemies(int amount, int layer, System.Action<EnemyBase> onSpawned)
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning($"[EnemySpawner] No hay spawnPoints en {name}");
            yield break;
        }

        List<Transform> pointsShuffled = spawnPoints.OrderBy(_ => Random.value).ToList();

      
        for (int i = 0; i < amount; i++)
        {
            Transform point = pointsShuffled[i % pointsShuffled.Count];

            //if (spawnMarkerPrefab != null)
            //{
            //    var marker = Instantiate(spawnMarkerPrefab, point.position, point.rotation);
            //    yield return new WaitForSeconds(preSpawnDelay);
            //    if (marker != null) Destroy(marker);
            //}

           
            string enemyId = GetEnemyIdForLayer(layer);
            EnemyBase enemy = GetFromPoolOrCreate(enemyId, point.position, point.rotation);

            if (enemy != null)
            {
                statScaler?.ApplyScaling(enemy, layer);

                // Bind de retorno al pool cuando muere
                var binder = enemy.GetComponent<EnemyPoolBinder>();
                if (binder == null) binder = enemy.gameObject.AddComponent<EnemyPoolBinder>();
                binder.Bind(this, enemyId, enemy);

                onSpawned?.Invoke(enemy);
            }

           
            yield return null;
        }
    }

    /// <summary>
    /// Limpia estado interno y desactiva todo lo poolleado.
    /// </summary>
    public void ResetSpawner()
    {
        foreach (var kvp in pools)
        {
            var q = kvp.Value;
            foreach (var e in q)
            {
                if (e != null) e.gameObject.SetActive(false);
            }
        }
    }

    internal void ReturnToPool(string enemyId, EnemyBase enemy)
    {
        if (enemy == null) return;
        enemy.gameObject.SetActive(false);

        if (!pools.TryGetValue(enemyId, out var q))
        {
            q = new Queue<EnemyBase>();
            pools[enemyId] = q;
        }
        q.Enqueue(enemy);
    }

    private EnemyBase GetFromPoolOrCreate(string enemyId, Vector3 pos, Quaternion rot)
    {
        EnemyBase instance = null;

        if (pools.TryGetValue(enemyId, out var q) && q.Count > 0)
        {
            instance = q.Dequeue();
            if (instance != null)
            {
                var t = instance.transform;
                t.SetPositionAndRotation(pos, rot);
                instance.gameObject.SetActive(true);
                return instance;
            }
        }

        // Instanciar nuevo usando la factory
        instance = enemyFactory.Create(enemyId, pos, rot);
        return instance;
    }

    // ---------- Selección por Capa (usa porcentajes de tu Spec) ----------
    private string GetEnemyIdForLayer(int layer)
    {
        const string RATA = "rat";
        const string LOBO = "wolf";
        const string OSO = "bear";
        const string CAZADOR = "hunter";
        const string CLERIGO = "cleric";
        const string PALADIN = "paladin";

        var weighted = new Dictionary<string, float>();

        if (layer <= 2)                             // Capas 1-2
        {
            weighted[RATA] = 50;
            weighted[LOBO] = 30;
            weighted[OSO] = 20;
        }
        else if (layer <= 4)                        // Capas 3-4
        {
            weighted[RATA] = 35;
            weighted[LOBO] = 35;
            weighted[OSO] = 25;
            weighted[CAZADOR] = 5;
        }
        else if (layer <= 6)                        // Capas 5-6
        {
            weighted[RATA] = 15;
            weighted[LOBO] = 15;
            weighted[OSO] = 30;
            weighted[CAZADOR] = 20;
            weighted[CLERIGO] = 20;
        }
        else                                        // Capas 7+
        {
            weighted[RATA] = 5;
            weighted[LOBO] = 5;
            weighted[OSO] = 15;
            weighted[CAZADOR] = 30;
            weighted[CLERIGO] = 30;
            weighted[PALADIN] = 15;
        }

        // Reutilizo tu ruleta si existe
        return RouletteSelection.Roulette(weighted);
    }
}
