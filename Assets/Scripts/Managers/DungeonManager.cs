
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum RoomSize { Small, Medium, Large }
public class DungeonManager : Singleton<DungeonManager>
{
    [Header("Player Reference")]
    public Transform player;

    [Header("Room Pools")]
    [SerializeField] private List<RoomController> smallRooms;
    [SerializeField] private List<RoomController> mediumRooms;
    [SerializeField] private List<RoomController> largeRooms;

    [Header("Run Settings")]
    [Tooltip("Cuántas salas recientes recordar para evitar repeticiones inmediatas")]
    public int historyLimit = 2;

    [Tooltip("Transform de inicio de dungeon")]
    public Transform startSpawnPoint;

    private List<RoomController> runSequence = new();
    private int currentRoomIndex = 0;
    private Queue<RoomController> recentHistory = new();

    private void Awake()
    {
        CreateSingleton(false);
    }
    private void Start()
    {
        GenerateRunSequence();
        StartRun();
    }
    private void GenerateRunSequence()
    {
        runSequence.Clear();
        recentHistory.Clear();

        var roomPlan = new List<(List<RoomController> pool, int amount)>
        {
            (smallRooms, 8),
            (mediumRooms, 6),
            (largeRooms, 4)
        };

        foreach (var (pool, amount) in roomPlan)
        {
            for (int i = 0; i < amount; i++)
            {
                var candidates = pool.Where(r => !recentHistory.Contains(r)).ToList();

                if (candidates.Count == 0)
                {
                    recentHistory.Clear();
                    candidates = pool;
                }

                var chosen = candidates[Random.Range(0, candidates.Count)];
                runSequence.Add(chosen);
                recentHistory.Enqueue(chosen);

                if (recentHistory.Count > historyLimit)
                    recentHistory.Dequeue();
            }
        }

        RouletteSelection.Shuffle(runSequence);
    }
    private void StartRun()
    {
        currentRoomIndex = 0;
        MovePlayerToRoom(runSequence[currentRoomIndex]);
    }


    private void MovePlayerToRoom(RoomController room)
    {
        Debug.Log($"Moviendo al jugador a la sala: {room.name}");

        player.position = room.EntryDoor.GetSpawnPoint();
        room.ActivateRoom();
    }

    private void TeleportPlayer(Vector3 targetPosition)
    {
        if (player == null)
        {
            Debug.LogError("Player no asignado en DungeonManager.");
            return;
        }

        player.position = targetPosition;
    }

    public void MoveToNextRoom()
    {
        currentRoomIndex++;

        if (currentRoomIndex >= runSequence.Count)
        {
            Debug.Log("Dungeon completado.");
            return;
        }

        MovePlayerToRoom(runSequence[currentRoomIndex]);
    }
    public RoomController GetCurrentRoom() => runSequence[currentRoomIndex];

    public void OnPlayerDeath()
    {
        Debug.Log("[DungeonManager] OnPlayerDeath llamado.");
        TeleportPlayer(startSpawnPoint.position);
        
    }
}