﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BearAI : EnemyBase
{
    [Header("Oso")]
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private float attackArea = 5f;

    [Header("Audio")]
    [SerializeField] private AudioClip atkClip;

    private float attackCooldownTimer = 0f;
    private bool isAttacking;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (isDead || isAttacking) return;

        if (IsInKnockback())
        {
            agent.isStopped = true;
            return;
        }

        HandlePerception();
        HandleMovementAndAttack();
    }

    #region Behavior Methods

    private bool IsInKnockback()
    {
        var kb = GetComponent<EnemyKnockback>();
        return kb != null && kb.IsActive;
    }

    private void HandlePerception()
    {
        attackCooldownTimer += Time.deltaTime;
        PerceptionUpdate();
    }

    private void HandleMovementAndAttack()
    {
        if (!CanSeePlayer)
        {
            StopAgent();
            return;
        }

        MoveTowardsPlayer();

        if (IsInAttackRange() && attackCooldownTimer >= enemyData.AttackCooldown)
        {
            StopAgent();
            StartCoroutine(CO_BearClawAttack());
        }
    }

    private bool IsInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) <= attackRange;
    }

    private void MoveTowardsPlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private void StopAgent()
    {
        agent.isStopped = true;
    }

    #endregion

    #region Attack Coroutine

    private IEnumerator CO_BearClawAttack()
    {
        isAttacking = true;

        yield return new WaitForSeconds(0.4f); // Simulación de animación de ataque

        PlayAttackSound();
        PerformAreaAttack();

        attackCooldownTimer = 0f;
        isAttacking = false;
    }

    private void PlayAttackSound()
    {
        if (atkClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(atkClip);
        }
    }

    private void PerformAreaAttack()
    {
        Vector3 attackCenter = transform.position + transform.forward * 2f;
        Collider[] hits = Physics.OverlapSphere(attackCenter, attackArea);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<IDamageable>()?.TakeDamage(enemyData.Damage);
            }
        }
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 2f, attackArea);
        LineOfSight.DrawLOSOnGizmos(transform, visionAngle, visionRange);
    }
#endif
}
