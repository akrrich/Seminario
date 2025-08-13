using UnityEngine;
public class DoorController : MonoBehaviour
{
    public DungeonManager dungeonManager;
    public RoomType targetType; // Combat o Hallway
    public bool isLocked { get; private set; } = false;

    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el que entra es el jugador
        if (other.CompareTag(playerTag))
        {
            if (!isLocked)
            {
                dungeonManager.TeleportToRandomRoom(targetType);
            }
            else
            {
                Debug.Log("La puerta está bloqueada.");
            }
        }
    }

    public void Lock()
    {
        isLocked = true;
       
    }

    public void Unlock()
    {
        isLocked = false;
        
    }
}