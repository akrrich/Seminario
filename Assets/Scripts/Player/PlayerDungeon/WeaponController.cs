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

    private void Update()
    {
        if (!CanAttack()) return;

        // Optional: Use PlayerInputs instead of hard-coded input
        if (PlayerInputs.Instance != null)
        {
            if (PlayerInputs.Instance.Attack())
            {
                PerformAttack();
            }
        }
        else
        {
            // Fallback for testing
            if (Input.GetMouseButtonDown(0))
            {
                PerformAttack();
            }
        }
    }

    private bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    public void PerformAttack()
    {
        lastAttackTime = Time.time;

        if (swordAnimator != null)
        {
            swordAnimator.SetTrigger("Attack");
        }
    }

    // Optional: For future hit detection support
}
