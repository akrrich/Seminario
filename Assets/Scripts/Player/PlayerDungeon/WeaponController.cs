using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private GameObject sword;
    [SerializeField] private float attackCooldown = 1f;

    private Animator swordAnimator;
    private float lastAttackTime = -Mathf.Infinity;

    private void Awake()
    {
        if (sword != null)
        {
            swordAnimator = sword.GetComponent<Animator>();
        }

        if (swordAnimator == null)
        {
            Debug.LogWarning("WeaponController: No Animator found on sword object.");
        }
    }

    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    public void PerformAttack()
    {
        if (!CanAttack()) return;

        lastAttackTime = Time.time;

        if (swordAnimator != null)
        {
            swordAnimator.SetTrigger("Attack");
        }
    }

    public float AttackCooldown => attackCooldown;
}
