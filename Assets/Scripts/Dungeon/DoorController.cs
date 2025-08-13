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
            Debug.Log("La puerta está bloqueada.");
        }
    }

    public void Lock()
    {
        isLocked = true;
        // Aquí podrías poner animación de cierre o sonido
    }

    public void Unlock()
    {
        isLocked = false;
        // Aquí podrías poner animación de apertura o sonido
    }
}