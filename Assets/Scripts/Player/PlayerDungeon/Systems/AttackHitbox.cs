using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private PlayerDungeonModel model;

    [Header("Hitbox Settings")]
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private Vector3 localOffset = new Vector3(0, 0, 1.2f);
    [SerializeField] private LayerMask targetLayer;

    [Header("Pivot")]
    [SerializeField] private Transform pivot;

    // NEW ---- Knockback tuning (expuestos para probar rápido en el Inspector)
    [Header("Knockback")]
    [SerializeField] private bool applyKnockback = true;
    [SerializeField] private float knockbackDistance = 1.0f;
    [SerializeField] private float knockbackDuration = 0.12f;
    [SerializeField] private float knockbackVerticalLift = 0.1f;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;

    private void Awake()
    {
        model = GetComponent<PlayerDungeonModel>();
        if (pivot == null)
            pivot = transform.Find("Orientation") ?? transform;
    }

    public void TriggerHit()
    {
        Vector3 worldCenter = pivot.TransformPoint(localOffset);
        Collider[] hits = Physics.OverlapSphere(worldCenter, radius, targetLayer); // 

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(model.CurrentWeaponDamage); // daña como antes
            }

            // NEW ---- si el enemigo tiene EnemyKnockback, lo empujamos
            if (applyKnockback && hit.TryGetComponent<EnemyKnockback>(out var kb))
            {
                Vector3 dir = hit.transform.position - worldCenter; // desde el centro de la esfera hacia el objetivo
                dir.y = 0f; // knockback plano (el lift lo da el propio componente)
                if (dir.sqrMagnitude < 0.0001f) dir = pivot.forward; // fallback si está encima
                kb.ApplyKnockback(dir.normalized, knockbackDistance, knockbackDuration, knockbackVerticalLift);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        Transform p = pivot != null ? pivot : transform;
        Vector3 center = p.TransformPoint(localOffset);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, radius);
    }
}
