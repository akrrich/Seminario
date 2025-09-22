using UnityEngine;

[CreateAssetMenu(
    fileName = "SpawnerConfig",
    menuName = "Dungeon/Spawner Config",
    order = 0)]
public class SpawnerConfigData : ScriptableObject
{
    [Header("Cantidad de enemigos por oleada")]
    [Tooltip("Cantidad mínima de enemigos que puede generar este spawner en una activación")]
    [SerializeField] private int minEnemiesPerSpawn = 1;

    [Tooltip("Cantidad máxima de enemigos que puede generar este spawner en una activación")]
    [SerializeField] private int maxEnemiesPerSpawn = 3;

    [Header("Control del spawner")]
    [Tooltip("Tiempo en segundos que debe pasar entre cada activación")]
    [SerializeField] private float spawnCooldown = 5f;

    [Tooltip("Cantidad total de enemigos que puede generar este spawner antes de apagarse")]
    [SerializeField] private int maxSpawnedEnemies = 10;

    [Tooltip("Tiempo en segundos que tarda el spawner en resetearse y volver a activarse")]
    [SerializeField] private float spawnerResetTime = 60f;

    // ---- GETTERS ----
    public int MinEnemiesPerSpawn => minEnemiesPerSpawn;
    public int MaxEnemiesPerSpawn => maxEnemiesPerSpawn;
    public float SpawnCooldown => spawnCooldown;
    public int MaxSpawnedEnemies => maxSpawnedEnemies;
    public float SpawnerResetTime => spawnerResetTime;
}