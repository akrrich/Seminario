using UnityEngine;
using System.Collections.Generic;

public class RoomController : MonoBehaviour
{
    [Header("Room Info")]
    [SerializeField] private RoomConfig config;

    [Header("Spawns")]
    [SerializeField] private EnemySpawner[] enemySpawners;

    [Header("Doors")]
    [SerializeField] private DoorController entryDoor;
    [SerializeField] private DoorController[] exitDoors;

    private readonly List<EnemyBase> enemies = new();

    public RoomConfig Config => config;
    public DoorController EntryDoor => entryDoor;
    public DoorController[] ExitDoors => exitDoors;
    public void ActivateRoom(int currentLayer)
    {
        Debug.Log($"[RoomController] Activando sala {config.roomID} en capa {currentLayer}");

        enemies.Clear();

        foreach (var spawner in enemySpawners)
        {
            if (spawner == null) continue;

            var spawned = spawner.SpawnEnemies(currentLayer);
            foreach (var enemy in spawned)
                RegisterEnemy(enemy);
        }

        LockAllExitDoors();
    }
    public void RegisterEnemy(EnemyBase enemy)
    {
        if (enemies.Contains(enemy)) return;

        enemies.Add(enemy);
        enemy.OnDeath += HandleEnemyDeath;
    }
    private void HandleEnemyDeath(EnemyBase enemy)
    {
        enemies.Remove(enemy);
        Debug.Log($"[{config.roomID}] Enemigo eliminado. Restantes: {enemies.Count}");

        if (enemies.Count == 0)
        {
            Debug.Log($"[{config.roomID}] Todos los enemigos fueron derrotados.");
            UnlockAllExitDoors();
        }
    }
    private void LockAllExitDoors()
    {
        if (exitDoors == null) return;

        foreach (var door in exitDoors)
        {
            if (door != null)
                door.Lock();
        }
    }

    private void UnlockAllExitDoors()
    {
        if (exitDoors == null) return;

        foreach (var door in exitDoors)
        {
            if (door != null)
                door.Unlock();
        }
    }
}
