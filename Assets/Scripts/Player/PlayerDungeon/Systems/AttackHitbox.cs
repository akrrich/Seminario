using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private PlayerDungeonModel model;

    [Header("Hitbox Settings")]
    [Tooltip("Radio de la esfera de impacto")]
    [SerializeField] private float radius = 1.5f;
    [Tooltip("Offset local desde el pivot")]
    [SerializeField] private Vector3 localOffset = new Vector3(0, 0, 1.2f);
    [Tooltip("Capa de objetivos que pueden ser dañados")]
    [SerializeField] private LayerMask targetLayer;

    [Header("Pivot")]
    [Tooltip("Transform desde el que se calcula el offset (usa Orientation)")]
    [SerializeField] private Transform pivot;

    [Header("Knockback")]
    [SerializeField] private bool applyKnockback = true;
    [SerializeField] private float knockbackDistance = 1.0f;
    [SerializeField] private float knockbackDuration = 0.12f;
    [SerializeField] private float knockbackVerticalLift = 0.10f;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;

    private void Awake()
    {
        model = GetComponent<PlayerDungeonModel>();
        if (pivot == null)
            pivot = transform.Find("Orientation") ?? transform;
    }

    /// <summary>Se llama desde la animación o desde CombatHandler.</summary>
    public void TriggerHit()
    {
        Vector3 worldCenter = pivot.TransformPoint(localOffset);
        Collider[] hits = Physics.OverlapSphere(worldCenter, radius, targetLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(model.CurrentWeaponDamage);
            }

            // Aplicar knockback si el objetivo lo soporta
            if (applyKnockback && hit.TryGetComponent<EnemyKnockback>(out var kb))
            {
                Vector3 dir = hit.transform.position - worldCenter;
                dir.y = 0f; // el lift lo maneja el propio componente
                if (dir.sqrMagnitude < 0.0001f) dir = pivot.forward; // fallback
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
