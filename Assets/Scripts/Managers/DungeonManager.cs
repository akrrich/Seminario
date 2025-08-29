using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : Singleton<DungeonManager>
{
    [Header("Player Reference")]
    [SerializeField] private Transform player;

    [Header("Dungeon Pools")]
    [SerializeField] private List<Transform> roomPrefabsList;     // 8 disponibles
    [SerializeField] private List<Transform> hallwayPrefabsList;  // 4 disponibles

    [Header("Dungeon Settings")]
    [SerializeField] private List<RoomController> rooms;

    [Header("Run Settings")]
    [SerializeField] private Transform startSpawnPoint;
    [SerializeField] private int historyLimit = 2;
    [SerializeField] private int totalRooms = 18;

    private Dictionary<string, Transform> roomPrefabs = new();
    private Dictionary<string, Transform> hallwayPrefabs = new();
    private List<Transform> runSequence = new();
    private int currentRoomIndex = 0;

    private Queue<Transform> recentRooms = new();
    private Queue<Transform> recentHallways = new();

    private RoomController currentRoom;
    private int currentLayer = 1;
    private bool runStarted = false;


    /* -------------------- PROPIEDADES P�BLICAS -------------------- */
    public Transform Player => player;
    public Transform StartSpawnPoint => startSpawnPoint;
    public int CurrentLayer => currentLayer;
    public bool RunStarted => runStarted;

    /* -------------------- M�TODOS P�BLICOS -------------------- */

    private void Awake()
    {
        CreateSingleton(false);
        InitializeDictionaries();
    }

    private void Start()
    {
        Debug.Log("[DungeonManager] Esperando que el player toque la primera puerta...");

        GenerateRunSequence();

        if (player != null && startSpawnPoint != null)
        {
            player.position = startSpawnPoint.position;
        }
    }
    /// <summary>
    /// Llamado cuando el player toca la primera puerta para iniciar la run
    /// </summary>
    public void StartDungeonRun()
    {
        if (runStarted) return;

        runStarted = true;
        Debug.Log("[DungeonManager] �Run iniciada! Player toc� la primera puerta.");

        StartRun();
    }
    public void EnterRoom(RoomController room)
    {
        if (room == null) return;

        currentRoom = room;
        Debug.Log($"[DungeonManager] Entrando a sala {room.Config.roomID} en Layer {CurrentLayer}");
        room.ActivateRoom(CurrentLayer);
    }

    public void MoveToNext()
    {
        currentRoomIndex++;

        if (currentRoomIndex >= runSequence.Count)
        {
            Debug.Log("[DungeonManager] Dungeon completado. Volviendo al Lobby.");
            TeleportPlayer(startSpawnPoint.position);
            return;
        }

        MovePlayerTo(runSequence[currentRoomIndex]);
    }

    public void OnPlayerDeath()
    {
        Debug.Log("[DungeonManager] OnPlayerDeath llamado. Reset de historiales.");
        TeleportPlayer(startSpawnPoint.position);
        ClearHistories();
    }

    /// <summary>
    /// Para asignar en un bot�n de UI --> manda al Lobby.
    /// </summary>
    public void TeleportToLobby()
    {
        Debug.Log("[DungeonManager] Teleport manual al Lobby desde UI.");
        TeleportPlayer(startSpawnPoint.position);
        ClearHistories();
    }
    public void AdvanceLayer()
    {
        currentLayer++;
        Debug.Log($"[DungeonManager] Avanzando a capa {currentLayer}");
    }
    /* -------------------- M�TODOS PRIVADOS -------------------- */

    private void InitializeDictionaries()
    {
        roomPrefabs.Clear();
        hallwayPrefabs.Clear();

        for (int i = 0; i < roomPrefabsList.Count; i++)
            roomPrefabs.Add("Room_" + i, roomPrefabsList[i]);

        for (int i = 0; i < hallwayPrefabsList.Count; i++)
            hallwayPrefabs.Add("Hallway_" + i, hallwayPrefabsList[i]);
    }

    private void GenerateRunSequence()
    {
        runSequence.Clear();
        recentRooms.Clear();
        recentHallways.Clear();
        currentLayer = 1;

        for (int i = 0; i < totalRooms; i++)
        {
            // --- Habitaci�n ---
            var room = GetRandomFromDict(roomPrefabs, recentRooms);
            runSequence.Add(room);
            AddToHistory(recentRooms, room);

            // --- Pasillo ---
            if (i < totalRooms - 1)
            {
                var hallway = GetRandomFromDict(hallwayPrefabs, recentHallways);
                runSequence.Add(hallway);
                AddToHistory(recentHallways, hallway);
            }

            // --- Aumentar capa cada 4 habitaciones ---
            if ((i + 1) % 4 == 0)
                currentLayer++;
        }
    }

    private void StartRun()
    {
        currentRoomIndex = 0;
        if (runSequence.Count == 0)
        {
            Debug.LogError("[DungeonManager] runSequence vac�o al iniciar la run.");
            return;
        }

        LoadRoomFromRunSequence(currentRoomIndex);
    }
    private void LoadRoomFromRunSequence(int index)
    {
        if (index >= runSequence.Count)
        {
            Debug.LogError("[DungeonManager] �ndice fuera de rango en runSequence.");
            return;
        }

        Transform roomPrefab = runSequence[index];
        Transform instance = Instantiate(roomPrefab);

        instance.position = currentRoom != null
            ? currentRoom.GetSpawnPoint().position
            : startSpawnPoint.position;

        MovePlayerTo(instance);

        RoomController controller = instance.GetComponent<RoomController>();
        if (controller != null)
        {
            EnterRoom(controller);
        }
        else
        {
            Debug.LogWarning("[DungeonManager] Sala sin RoomController.");
        }
    }
    private void MovePlayerTo(Transform target)
    {
        currentRoomIndex++;

        if (player == null)
        {
            Debug.LogError("Player no asignado en DungeonManager.");
            return;
        }

        if (currentRoomIndex >= runSequence.Count)
        {
            Debug.Log("[DungeonManager] Dungeon completado. Volviendo al Lobby.");
            TeleportPlayer(startSpawnPoint.position);
            return;
        }

        Debug.Log($"[DungeonManager] Moviendo al jugador a: {target.name}");
        player.position = target.position;
    }

    private void TeleportPlayer(Vector3 targetPosition)
    {
        if (player == null) return;
        player.position = targetPosition;
    }

    private void ClearHistories()
    {
        recentRooms.Clear();
        recentHallways.Clear();
        runSequence.Clear();
        currentRoomIndex = 0;
        currentLayer = 1;
    }

    private Transform GetRandomFromDict(Dictionary<string, Transform> dict, Queue<Transform> history)
    {
        List<Transform> candidates = new List<Transform>(dict.Values);
        
        candidates.RemoveAll(r => history.Contains(r));

        if (candidates.Count == 0)
        {
            history.Clear();
            candidates = new List<Transform>(dict.Values);
        }

        candidates = RouletteSelection.Shuffle(candidates);

        return candidates[0]; // Tomamos el primero despu�s del shuffle
    }

    private void AddToHistory(Queue<Transform> history, Transform item)
    {
        history.Enqueue(item);
        if (history.Count > historyLimit)
            history.Dequeue();
    }
}
