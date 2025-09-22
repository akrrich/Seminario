using UnityEngine;
using System.Collections;

public class SpikeTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private int damage = 20;
    [SerializeField] private float spikeDistance = 1.5f;
    [SerializeField] private float spikeSpeed = 5f;
    [SerializeField] private float resetDelay = 1f;
    [SerializeField] private float cooldownTime = 2f;

    [Header("Timing")]
    [SerializeField] private float damageDelay = 0.5f; // <-- NUEVO: ventana para esquivar

    [Header("Dirección del disparo (local)")]
    [SerializeField] private Vector3 spikeDirection = Vector3.up; // piso: up | techo: down | pared: forward/left/right

    [Header("References")]
    [SerializeField] private Transform spikeMesh;  // el objeto que se mueve

    private Vector3 originalLocalPos;
    private bool isActive = false;
    private float cooldownTimer = 0f;

    // control de daño diferido
    private PlayerDungeonModel currentTarget;      // jugador que activó la trampa
    private Coroutine pendingDamageRoutine;        // para cancelar si sale del trigger

    private void Start()
    {
        if (!spikeMesh)
        {
            Debug.LogError("SpikeTrap: Asigna 'spikeMesh' en el inspector.");
            enabled = false;
            return;
        }
        originalLocalPos = spikeMesh.localPosition;
    }

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActive || cooldownTimer > 0f) return;

        if (other.TryGetComponent<PlayerDungeonModel>(out var player))
        {
            // activamos la trampa (mueve pinchos ya)
            isActive = true;
            currentTarget = player;

            // mover pinchos hacia afuera según dirección local configurable
            Vector3 worldDir = transform.TransformDirection(spikeDirection.normalized);
            Vector3 targetLocalPos = originalLocalPos + spikeMesh.InverseTransformDirection(worldDir) * spikeDistance;

            StartCoroutine(SpikeMovement(targetLocalPos));

            // programar daño diferido
            pendingDamageRoutine = StartCoroutine(DelayedDamage());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // si el mismo jugador sale antes del delay, se cancela el daño
        if (currentTarget != null && other.GetComponent<PlayerDungeonModel>() == currentTarget)
        {
            if (pendingDamageRoutine != null)
            {
                StopCoroutine(pendingDamageRoutine);
                pendingDamageRoutine = null;
            }
            currentTarget = null; // queda activada pero sin objetivo: no hace daño esta vez
        }
    }

    private IEnumerator DelayedDamage()
    {
        yield return new WaitForSeconds(damageDelay);

        // sólo daña si el objetivo sigue siendo válido (sigue dentro)
        if (currentTarget != null)
        {
            currentTarget.TakeDamage(damage);
        }

        pendingDamageRoutine = null;
    }

    private IEnumerator SpikeMovement(Vector3 targetLocalPos)
    {
        // ida
        Vector3 start = spikeMesh.localPosition;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * spikeSpeed;
            spikeMesh.localPosition = Vector3.Lerp(start, targetLocalPos, t);
            yield return null;
        }

        // pequeño tiempo extendida
        yield return new WaitForSeconds(resetDelay);

        // vuelta
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * spikeSpeed;
            spikeMesh.localPosition = Vector3.Lerp(targetLocalPos, originalLocalPos, t);
            yield return null;
        }

        // limpieza / cooldown
        cooldownTimer = cooldownTime;
        isActive = false;
        currentTarget = null;

        if (pendingDamageRoutine != null)
        {
            StopCoroutine(pendingDamageRoutine); // por las dudas
            pendingDamageRoutine = null;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // visualiza la dirección local configurada
        Gizmos.color = Color.red;
        Vector3 p = spikeMesh ? spikeMesh.position : transform.position;
        Vector3 dir = transform.TransformDirection(spikeDirection.normalized);
        Gizmos.DrawLine(p, p + dir * 1.0f);
        Gizmos.DrawSphere(p + dir * 1.0f, 0.05f);
    }
#endif
}
