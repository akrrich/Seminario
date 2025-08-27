using UnityEngine;
public class DoorController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openTrigger = "Open";
    [SerializeField] private string closeTrigger = "Close";

    [Header("Collision")]
    [SerializeField] private Collider doorCollider;

    [Header("VFX")]
    [SerializeField] private ParticleSystem openVFX;
    [SerializeField] private ParticleSystem closeVFX;

    [Header("Room Connection")]
    [SerializeField] private RoomController connectedRoom;
    [SerializeField] private bool isExitDoor = true;
    [SerializeField] private bool isFirstDoor = false;

    private bool isLocked = true;

    public void Unlock()
    {
        if (!isLocked) return;

        isLocked = false;

        // Play opening animation
        if (doorAnimator != null)
            doorAnimator.SetTrigger(openTrigger);

        // Disable collision
        if (doorCollider != null)
            doorCollider.enabled = false;

        // Play VFX
        if (openVFX != null)
            openVFX.Play();

        Debug.Log("[DoorController] Puerta desbloqueada.");
    }

    public void Lock()
    {
        if (isLocked) return;

        isLocked = true;

        // Play closing animation
        if (doorAnimator != null)
            doorAnimator.SetTrigger(closeTrigger);

        // Enable collision
        if (doorCollider != null)
            doorCollider.enabled = true;

        // Play VFX
        if (closeVFX != null)
            closeVFX.Play();

        Debug.Log("[DoorController] Puerta bloqueada.");
    }

    public Vector3 GetSpawnPoint() => transform.position;

    private void OnTriggerEnter(Collider other)
    {
        if (!isExitDoor || isLocked) return;

        if (other.CompareTag("Player"))
        {
            if (isFirstDoor && !DungeonManager.Instance.RunStarted)
            {
                DungeonManager.Instance.StartDungeonRun();
                return;
            }

            if (connectedRoom != null)
            {
                // Enter the connected room
                DungeonManager.Instance.EnterRoom(connectedRoom);
            }
            else
            {
                // Move to next room in sequence
                DungeonManager.Instance.MoveToNext();
            }
        }
    }

    // Property to check if door is locked
    public bool IsLocked => isLocked;

    // Método para marcar esta puerta como la primera
    public void SetAsFirstDoor(bool firstDoor)
    {
        isFirstDoor = firstDoor;
    }
}