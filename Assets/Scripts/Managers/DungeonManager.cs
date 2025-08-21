
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum RoomType { Combat, Hallway }
public enum RoomSize { Small, Medium, Large }
public class DungeonManager : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player;

    [Header("Death Respawn")]
    [Tooltip("Posición a la que será enviado el jugador al morir")]
    public Transform deathSpawnPoint;


    [Header("Combat Room Pools")]
    public List<Transform> smallRooms;
    public List<Transform> mediumRooms;
    public List<Transform> largeRooms;

    [Header("Hallway Pool")]
    public List<Transform> hallways;

    [Header("Random Settings")]
    [Tooltip("Cuántas salas recientes recordar para evitar repeticiones inmediatas")]
    public int historyLimit = 3;

    private Transform previousRoom;
    private Queue<Transform> recentRooms = new Queue<Transform>();

    /// <summary>
    /// Teletransporta al jugador a una sala o pasillo aleatorio, evitando repeticiones cercanas.
    /// </summary>
    public void TeleportToRandomRoom(RoomType type)
    {
        List<Transform> pool = type == RoomType.Combat ? GetCombatRoomPool() : hallways;
        Transform target = GetNonRepeatingRoom(pool);

        if (target == null)
        {
            Debug.LogError($"No hay {type} disponibles para teletransporte.");
            return;
        }

        TeleportPlayer(target.position);
        previousRoom = target;
    }

    /// <summary>
    /// Combina todas las salas de combate disponibles.
    /// </summary>
    private List<Transform> GetCombatRoomPool()
    {
        var allRooms = new List<Transform>();
        allRooms.AddRange(smallRooms);
        allRooms.AddRange(mediumRooms);
        allRooms.AddRange(largeRooms);
        return allRooms;
    }

    /// <summary>
    /// Selecciona una sala que no esté en el historial reciente.
    /// </summary>
    private Transform GetNonRepeatingRoom(List<Transform> pool)
    {
        var filtered = pool.Where(r => !recentRooms.Contains(r)).ToList();

        // Si se agotaron las opciones únicas, permitimos repetir
        if (filtered.Count == 0)
            filtered = pool;

        // Usamos tu clase RouletteSelection para elegir al azar
        Transform chosen = RouletteSelection.Shuffle(filtered).First();

        // Actualizamos historial
        recentRooms.Enqueue(chosen);
        if (recentRooms.Count > historyLimit)
            recentRooms.Dequeue();

        return chosen;
    }

    /// <summary>
    /// Mueve al jugador a la posición de destino.
    /// </summary>
    private void TeleportPlayer(Vector3 targetPosition)
    {
        player.position = targetPosition;
    }

    public void OnPlayerDeath()
    {
        if (deathSpawnPoint == null)
        {
            Debug.LogError("No se ha asignado un deathSpawnPoint en DungeonManager");
            return;
        }

        TeleportPlayer(deathSpawnPoint.position);
        Debug.Log("Jugador respawneado en deathSpawnPoint");
    }
}