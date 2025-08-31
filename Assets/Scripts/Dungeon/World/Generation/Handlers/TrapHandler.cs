using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] trapPrefabs;
    private GameObject[] spawnedTraps;
    public void SpawnTraps(RoomConfig config, int layer)
    {
        int trapCount = GetTrapCountByRoomSize(config.size);
        spawnedTraps = new GameObject[trapCount];

        for (int i = 0; i < trapCount; i++)
        {
            Vector3 spawnPos = GetRandomTrapPosition();
            GameObject trapPrefab = trapPrefabs[Random.Range(0, trapPrefabs.Length)];
            spawnedTraps[i] = Instantiate(trapPrefab, spawnPos, Quaternion.identity, transform);
        }

        Debug.Log($"[TrapHandler] Spawned {trapCount} traps.");
    }
    public void Cleanup()
    {
        if (spawnedTraps == null) return;

        foreach (var trap in spawnedTraps)
        {
            if (trap != null)
                Destroy(trap);
        }

        spawnedTraps = null;
    }
    private int GetTrapCountByRoomSize(RoomSize size)
    {
        switch (size)
        {
            case RoomSize.Small: return 1;
            case RoomSize.Medium: return 2;
            case RoomSize.Large: return 3;
            default: return 1;
        }
    }
    private Vector3 GetRandomTrapPosition()
    {
        return transform.position + new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));
    }
}
