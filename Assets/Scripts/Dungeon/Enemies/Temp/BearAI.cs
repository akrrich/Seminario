using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BearAI : EnemyBase
{
    [Header("Oso")]
    [SerializeField] private float attackRange = 4f; // Distancia para atacar
    [SerializeField] private float attackArea = 5f;  // Área del ataque frontal
    [Space(2)]
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

        // ⬇️ Bloque nuevo: pausa la IA durante el knockback
        var kb = GetComponent<EnemyKnockback>();
        if (kb != null && kb.IsActive)
        {
            agent.isStopped = true;
            return;
        }

        attackCooldownTimer += Time.deltaTime;

        PerceptionUpdate();

        if (!CanSeePlayer)
        {
            agent.isStopped = true;
            return;
        }

        // Movimiento hacia el jugador
        agent.isStopped = false;
        agent.SetDestination(player.position);

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange && attackCooldownTimer >= enemyData.AttackCooldown)
        {
            agent.isStopped = true;
            StartCoroutine(BearClawAttack());
        }
    }

    private IEnumerator BearClawAttack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.4f); // simula animación
        audioSource.PlayOneShot(atkClip);

        // Ataque en área grande frente a él
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 2f, attackArea);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
                hit.GetComponent<IDamageable>()?.TakeDamage(enemyData.Damage);
        }

        attackCooldownTimer = 0f;
        isAttacking = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyData.DistanceToPlayer);
        LineOfSight.DrawLOSOnGizmos(transform, visionAngle, visionRange);
    }
#endif
}
