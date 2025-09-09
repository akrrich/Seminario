using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float spikeDistance = 1.5f;
    [SerializeField] private float spikeSpeed = 5f;
    [SerializeField] private float resetDelay = 1f;
    [SerializeField] private float cooldownTime = 2f;

    [Header("Dirección del disparo (local)")]
    [SerializeField] private Vector3 spikeDirection = Vector3.up;


    [Header("References")]
    [SerializeField] private Transform spikeMesh;  // El objeto que se mueve

    private Vector3 originalPosition;
    private bool isActive = false;
    private float cooldownTimer = 0f;

    private void Start()
    {
        if (spikeMesh == null)
        {
            Debug.LogError("SpikeTrap: Falta referencia a 'spikeMesh'.");
            return;
        }

        originalPosition = spikeMesh.localPosition;
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActive || cooldownTimer > 0f) return;

        if (other.TryGetComponent<PlayerDungeonModel>(out var player))
        {
            ActivateTrap(player);
        }
    }

    private void ActivateTrap(PlayerDungeonModel player)
    {
        isActive = true;

        // Hacer daño
        player.TakeDamage((int)damage);

        // Mover pinchos hacia adelante
        Vector3 spikeTarget = originalPosition + transform.TransformDirection(spikeDirection.normalized) * spikeDistance;
        StartCoroutine(SpikeMovement(spikeTarget));
    }

    private System.Collections.IEnumerator SpikeMovement(Vector3 targetPosition)
    {
        Vector3 startPos = spikeMesh.localPosition;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * spikeSpeed;
            spikeMesh.localPosition = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }

        yield return new WaitForSeconds(resetDelay);

        // Volver a posición original
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * spikeSpeed;
            spikeMesh.localPosition = Vector3.Lerp(targetPosition, originalPosition, t);
            yield return null;
        }

        cooldownTimer = cooldownTime;
        isActive = false;
    }
}
