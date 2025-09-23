using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : Singleton<DungeonManager>
{
    [Header("Player Reference")]
    [SerializeField] private Transform player;

    [Header("Dungeon Pools")]
    [SerializeField] private List<Transform> roomPrefabsList;     // 8 disponibles
    [SerializeField] private List<Transform> hallwayPrefabsList;  // 4 disponibles

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

    /* -------------------- UNITY -------------------- */

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
        PlayerDungeonHUD.OnLayerChanged?.Invoke(currentLayer);
    }
    /* ------------------ API ---------------------- */
   
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
        room.ActivateRoom();
    }

    public void OnRoomCleared(RoomController clearedRoom)
    {
        Debug.Log($"[DungeonManager] Room {clearedRoom.Config.roomID} cleared. Moviendo a la siguiente sala...");
        if ((currentRoomIndex + 1) % 4 == 0 && currentRoomIndex + 1 < totalRooms )
        {
            AdvanceLayer();
        }
    }

    public void MoveToNext()
    {
        
        if (currentRoom != null)
        {
            Debug.Log($"[DungeonManager] Reiniciando la sala {currentRoom.Config.roomID} antes de avanzar.");
            currentRoom.ResetRoom();
        }

        currentRoomIndex++;
        
        if (currentRoomIndex >= runSequence.Count)
        {
            Debug.Log("[DungeonManager] Dungeon completado. Volviendo al Lobby.");
            TeleportPlayer(startSpawnPoint.position);
            return;
        }

        LoadRoomFromRunSequence(currentRoomIndex);
    }

    public void OnPlayerDeath()
    {
        Debug.Log("[DungeonManager] OnPlayerDeath llamado. Reset de historiales.");
        TeleportPlayer(startSpawnPoint.position);
        ClearHistories();
    }
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
        PlayerDungeonHUD.OnLayerChanged?.Invoke(currentLayer);
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
        PlayerDungeonHUD.OnLayerChanged?.Invoke(currentLayer);
        LoadRoomFromRunSequence(currentRoomIndex);
    }
    private void LoadRoomFromRunSequence(int index)
    {
        if (index >= runSequence.Count)
        {
            Debug.LogError("[DungeonManager] �ndice fuera de rango en runSequence.");
            return;
        }

        Transform targetSpawn = runSequence[index];
        MovePlayerTo(targetSpawn);

        RoomController controller = targetSpawn.GetComponentInParent<RoomController>();
        if (controller != null)
        {
            EnterRoom(controller);
        }
           
    }
    private void MovePlayerTo(Transform target)
    {
        if (player == null)
        {
            Debug.LogError("Player no asignado en DungeonManager.");
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
