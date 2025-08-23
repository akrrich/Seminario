using UnityEngine;
using System.Collections.Generic;

public class RoomController : MonoBehaviour
{
    [Header("Room Info")]
    [SerializeField] private RoomConfig config;
    [SerializeField] private int layer;

    [Header("Spawns")]
    [SerializeField] private EnemySpawner[] enemySpawners;

    [Header("Doors")]
    [SerializeField] private DoorController entryDoor;
    [SerializeField] private DoorController exitDoor;

    private readonly List<GameObject> enemies = new();

    public RoomConfig Config => config;
    public DoorController EntryDoor => entryDoor;
    public DoorController ExitDoor => exitDoor;

    public void ActivateRoom()
    {
        Debug.Log($"[RoomController] Activando sala {config.roomID} en capa {layer}");

        enemies.Clear();

        foreach (var spawner in enemySpawners)
        {
            if (spawner == null) continue;

            var spawned = spawner.SpawnEnemies(layer);
            foreach (var enemy in spawned)
            {
                RegisterEnemy(enemy.gameObject);
            }
        }

        exitDoor?.Lock();
    }
    private void RegisterEnemy(GameObject enemy)
    {
        if (enemies.Contains(enemy)) return;

        enemies.Add(enemy);

        var enemyBase = enemy.GetComponent<EnemyBase>();
        if (enemyBase != null)
        {
            enemyBase.OnDeath += HandleEnemyDeath;
        }
    }
    private void HandleEnemyDeath(EnemyBase enemy)
    {
        enemies.Remove(enemy.gameObject);
        Debug.Log($"[{config.roomID}] Enemigo eliminado. Restantes: {enemies.FindAll(e => e != null).Count}");

        if (enemies.TrueForAll(e => e == null))
        {
            Debug.Log($"[{config.roomID}] Todos los enemigos fueron derrotados.");
            exitDoor?.Unlock();
        }
    }
}
