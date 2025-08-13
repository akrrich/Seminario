using UnityEngine;
public class DoorController : MonoBehaviour, IInteractable
{
    public DungeonManager dungeonManager;
    public RoomType targetType; // Combat o Hallway
    public bool isLocked { get; private set; } = false;

    public void Interact()
    {
        if (!isLocked)
        {
            dungeonManager.TeleportToRandomRoom(targetType);
        }
        else
        {
            Debug.Log("La puerta est� bloqueada.");
        }
    }

    public void Lock()
    {
        isLocked = true;
        // Aqu� podr�as poner animaci�n de cierre o sonido
    }

    public void Unlock()
    {
        isLocked = false;
        // Aqu� podr�as poner animaci�n de apertura o sonido
    }
}