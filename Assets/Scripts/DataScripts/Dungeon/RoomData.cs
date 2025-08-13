using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRoomData", menuName = "Dungeon/Room Data")]
public class RoomData : ScriptableObject
{
    public string roomID;
    public RoomSize size;

    [Header("Runtime References")]
    public List<GameObject> enemies = new List<GameObject>();
    public DoorController entryDoor;
    public DoorController exitDoor;

    public int RemainingEnemies => enemies.FindAll(e => e != null).Count;

    public System.Action onAllEnemiesDefeated;

    public void Initialize()
    {
        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            var enemyBase = enemy.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                enemyBase.roomData = this;
            }
        }

        exitDoor?.Lock();
    }

    public void NotifyEnemyDied(GameObject enemy)
    {
        if (!enemies.Contains(enemy)) return;

        enemies.Remove(enemy);
        Debug.Log($"[{roomID}] Enemigo eliminado. Restantes: {RemainingEnemies}");

        if (RemainingEnemies == 0)
        {
            Debug.Log($"[{roomID}] Todos los enemigos fueron derrotados.");
            exitDoor?.Unlock();
            onAllEnemiesDefeated?.Invoke();
        }
    }
}
