using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Room Configuration")]
    [SerializeField] private RoomConfig config;

    [Header("Handlers")]
    [SerializeField] private EnemyHandler enemyHandler;
    [SerializeField] private LootHandler lootHandler;
    [SerializeField] private TrapHandler trapHandler;

    [Header("Doors")]
    [SerializeField] private DoorController entryDoor;
    [SerializeField] private DoorController[] exitDoors;

    [Header("Enemy Management")]
    [SerializeField] private List<EnemyBase> enemiesInRoom = new List<EnemyBase>();
    [SerializeField] private bool allEnemiesDefeated = false;

    [Header("References")]
    [SerializeField] private Transform roomSpawnPoint;

    private bool isActive = false;
    private int currentLayer;

    // ---------- PROPERTIES ----------
    public RoomConfig Config => config;
    public bool IsActive => isActive;
    public bool IsCleared => enemyHandler != null && enemyHandler.EnemyCount == 0;
    public Transform SpawnPoint => roomSpawnPoint;

    public event Action onAllEnemiesDefeated;
    //--------------- UNITY -------------------
    private void Awake()
    {
        InitializeHandlers();
    }
    //------------------ API ------------------
    private void InitializeHandlers()
    {
        if (enemyHandler == null) enemyHandler = GetComponentInChildren<EnemyHandler>();
        if (lootHandler == null) lootHandler = GetComponentInChildren<LootHandler>();
        if (trapHandler == null) trapHandler = GetComponentInChildren<TrapHandler>();

        enemyHandler?.Initialize();
    }

    public void ActivateRoom(int layer)
    {
        if (isActive) return;

        currentLayer = layer;
        isActive = true;

        Debug.Log($"[RoomController] Activating room {config.roomID} on layer {layer}");

        if (enemyHandler != null)
        {
            enemyHandler.OnAllEnemiesDefeated += HandleRoomCleared;
            enemyHandler.SpawnEnemies(config, layer);
        }

        if (config.allowLoot)
            lootHandler?.SpawnLoot(config.size, layer);

        if (config.allowTraps)
            trapHandler?.SpawnTraps(config, layer);

        LockExitDoors();
        entryDoor?.Unlock();
    }

    public void DeactivateRoom()
    {
        if (!isActive) return;

        isActive = false;

        if (enemyHandler != null)
        {
            enemyHandler.OnAllEnemiesDefeated -= HandleRoomCleared;
            enemyHandler.Cleanup();
        }

        lootHandler?.Cleanup();
        trapHandler?.Cleanup();

        UnlockExitDoors();
    }

    public void RegisterEnemies(EnemyBase enemy)
    {
        if (enemy == null || enemiesInRoom.Contains(enemy)) return;

        enemiesInRoom.Add(enemy);

        enemy.OnDeath += HandleEnemyDeath;

        Debug.Log($"[RoomController] Enemigo registrado: {enemy.name}. Total: {enemiesInRoom.Count}");
    }

    private void HandleEnemyDeath(EnemyBase enemy)
    {
        if (enemy == null) return;

        enemy.OnDeath -= HandleEnemyDeath;
          
        if (enemiesInRoom.Contains(enemy))
        {
            enemiesInRoom.Remove(enemy);
        }

        Debug.Log($"[RoomController] Enemigo derrotado. Restantes: {enemiesInRoom.Count}");

        // Verificar si todos los enemigos fueron derrotados
        CheckIfRoomCleared();
    }

    public void HandleAllEnemiesDefeated()
    {
        onAllEnemiesDefeated?.Invoke();
        Debug.Log("[RoomController] ¡Sala limpiada!");
    }

    private void CheckIfRoomCleared()
    {
        if (enemiesInRoom.Count == 0 && !allEnemiesDefeated)
        {
            allEnemiesDefeated = true;
            HandleAllEnemiesDefeated();
            Debug.Log("[RoomController] ¡Sala limpiada!");
        }
    }
    public bool IsRoomCleared()
    {
        return allEnemiesDefeated;
    }
    private void OnDestroy()
    {
        foreach (var enemy in enemiesInRoom)
        {
            if (enemy != null)
            {
                enemy.OnDeath -= HandleEnemyDeath;
            }
        }
        enemiesInRoom.Clear();
    }
    private void HandleRoomCleared()
    {
        Debug.Log($"[RoomController] Room {config.roomID} cleared!");
        UnlockExitDoors();
        DungeonManager.Instance?.OnRoomCleared(this);
    }

    private void ConfigureEntryDoor()
    {
        entryDoor?.Unlock();
    }

    private void LockExitDoors()
    {
        foreach (var door in exitDoors)
            door?.Lock();
    }

    private void UnlockExitDoors()
    {
        foreach (var door in exitDoors)
            door?.Unlock();
    }

    public void ResetRoom()
    {
        DeactivateRoom();
        InitializeHandlers();
    }
}
