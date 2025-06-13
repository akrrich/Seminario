using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private PlayerDungeonModel model;
    [Header("Hitbox Settings")]
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private Vector3 offset = Vector3.forward;
    [SerializeField] private LayerMask targetLayer;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;

    private Transform hitOrigin;

    private void Awake()
    {
        hitOrigin = transform;
        model = GetComponent<PlayerDungeonModel>();
    }

    public void TriggerHit()
    {
        Vector3 hitPoint = hitOrigin.position + hitOrigin.TransformDirection(offset);
        Collider[] hits = Physics.OverlapSphere(hitPoint, radius, targetLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(model.CurrentWeaponDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        Vector3 center = Application.isPlaying
            ? hitOrigin.position + hitOrigin.TransformDirection(offset)
            : transform.position + transform.TransformDirection(offset);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, radius);
    }
}
