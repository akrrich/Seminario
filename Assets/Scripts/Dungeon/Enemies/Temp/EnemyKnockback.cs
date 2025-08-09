using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyKnockback : MonoBehaviour
{
    [Header("Tuning")]
    [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private NavMeshAgent agent;
    private Coroutine routine;
    private bool active;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Aplica un empujón corto en dirección XZ, con un leve lift en Y.
    /// - dir: dirección del golpe (se normaliza y se aplana a XZ)
    /// - distance: metros a desplazar en horizontal
    /// - duration: tiempo total del empujón
    /// - verticalLift: cuánto subir en Y en el pico del empujón (0.05–0.15 va bien)
    /// </summary>
    public void ApplyKnockback(Vector3 dir, float distance, float duration, float verticalLift)
    {
        if (!isActiveAndEnabled) return;

        // cancelamos si hay uno previo
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(KnockRoutine(dir, distance, duration, verticalLift));
    }

    private IEnumerator KnockRoutine(Vector3 dir, float distance, float duration, float verticalLift)
    {
        active = true;

        // Preparación
        Vector3 startPos = transform.position;

        // aplanar y normalizar (solo XZ)
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) dir = transform.forward;
        dir.Normalize();

        // objetivo en XZ
        Vector3 desiredXZ = startPos + dir * distance;

        // clamp al NavMesh (por si hay pared/obstáculo)
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(desiredXZ, out navHit, 1.0f, NavMesh.AllAreas))
            desiredXZ = navHit.position;

        // levantamos un poco en Y (se siente mejor el impacto)
        Vector3 endPos = new Vector3(desiredXZ.x, startPos.y, desiredXZ.z);

        // congelamos el agent para mover a mano
        bool prevStopped = agent.isStopped;
        bool prevUpdatePos = agent.updatePosition;
        agent.isStopped = true;
        agent.updatePosition = false;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / duration);
            float k = ease.Evaluate(u);

            // Lerp horizontal + pequeño arco vertical (sube y baja)
            float yArc = Mathf.Sin(u * Mathf.PI) * verticalLift;

            Vector3 pos = Vector3.Lerp(startPos, endPos, k);
            pos.y = startPos.y + yArc;

            transform.position = pos;
            yield return null;
        }

        // asentamos al NavMesh (corrige Y si hace falta)
        if (NavMesh.SamplePosition(transform.position, out navHit, 1.0f, NavMesh.AllAreas))
        {
            agent.Warp(navHit.position);
        }
        else
        {
            agent.Warp(transform.position);
        }

        // restauramos el agent
        agent.updatePosition = prevUpdatePos;
        agent.isStopped = prevStopped;

        active = false;
        routine = null;
    }
}
