using UnityEngine;
public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Collider doorCollider;
    [SerializeField] private string openTrigger = "Open";

    public void Unlock()
    {
        // Activa la animaci�n de abrir
        if (doorAnimator != null)
            doorAnimator.SetTrigger(openTrigger);

        // Desactiva el collider si ya no bloquea f�sicamente
        if (doorCollider != null)
            doorCollider.enabled = false;
               
        Debug.Log("[DoorController] Puerta desbloqueada.");
    }

    public void Lock()
    {
        // Opcional: si en el futuro quer�s cerrarla din�micamente
    }
    public Vector3 GetSpawnPoint() => transform.position;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DungeonManager.Instance.MoveToNextRoom();
        }
    }
}