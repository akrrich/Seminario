using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Tooltip("Controla todas las rondas de enemigos de una sala. Coordina varios EnemySpawner y notifica al RoomController.")]
public class EnemyHandler : MonoBehaviour
{
    [Header("Spawners de la sala")]
    [Tooltip("Lista de EnemySpawner que se usan en la sala. Si se deja vacío, se auto-detectan en los hijos del Room.")]
    [SerializeField] private List<EnemySpawner> spawners = new();

    [Header("Rondas")]
    [Tooltip("Cantidad total de rondas en esta sala. Ejemplo: 3 --> (4, 8, 12 enemigos)")]
    [SerializeField] private int totalRounds = 3;

    [Tooltip("Número base de enemigos en la primera ronda. Cada ronda multiplica este número por el índice de ronda. Ejemplo: base = 4 --> ronda 1 = 4, ronda 2 = 8, ronda 3 = 12.")]
    [SerializeField] private int basePerRound = 4;  // 4, 8, 12...

    public event Action OnAllEnemiesDefeated;

    private int currentLayer;
    private int aliveCount;
    private bool initialized;

    public void Initialize(RoomConfig config, int layer)
    {
        currentLayer = layer;
        aliveCount = 0;
        initialized = true;

        if (spawners == null || spawners.Count == 0)
        {
            spawners = new List<EnemySpawner>(GetComponentsInChildren<EnemySpawner>(true));
        }

        StopAllCoroutines();
        StartCoroutine(RunRounds());
    }

    public void Cleanup()
    {
        StopAllCoroutines();
        foreach (var s in spawners)
            s?.ResetSpawner();

        initialized = false;
        aliveCount = 0;
    }

    private IEnumerator RunRounds()
    {
        if (!initialized || spawners == null || spawners.Count == 0)
        {
            Debug.LogWarning("[EnemyHandler] No hay spawners configurados en la sala.");
            yield break;
        }

        for (int round = 1; round <= totalRounds; round++)
        {
            int toSpawn = basePerRound * round; // 4, 8, 12...
            yield return SpawnRound(toSpawn);

            // Esperar a que mueran todos los enemigos antes de la siguiente ronda
            yield return new WaitUntil(() => aliveCount <= 0);
        }

        OnAllEnemiesDefeated?.Invoke();
    }

    private IEnumerator SpawnRound(int totalToSpawn)
    {
        if (totalToSpawn <= 0) yield break;

        int spawnerCount = spawners.Count;
        int basePerSpawner = totalToSpawn / spawnerCount;
        int remainder = totalToSpawn % spawnerCount;

        var coroutines = new List<Coroutine>();

        for (int i = 0; i < spawnerCount; i++)
        {
            int countForThis = basePerSpawner + (i < remainder ? 1 : 0);
            if (countForThis <= 0) continue;

            var s = spawners[i];
            Coroutine c = StartCoroutine(s.SpawnEnemies(countForThis, currentLayer, OnEnemySpawned));
            coroutines.Add(c);
        }

        // Esperar a que todos los spawners terminen de soltar su tanda
        foreach (var c in coroutines)
            yield return c;
    }

    private void OnEnemySpawned(EnemyBase enemy)
    {
        if (enemy == null) return;

        aliveCount++;

        // Asegurarse de desuscribirse
        enemy.OnDeath -= HandleEnemyDeath;
        enemy.OnDeath += HandleEnemyDeath;
    }

    private void HandleEnemyDeath(EnemyBase e)
    {
        e.OnDeath -= HandleEnemyDeath;
        aliveCount--;
    }
}
