using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class WolfAI : EnemyBase
{
    [Header("Lobo")]
    [SerializeField] private float attackRange = 5f; // área de impacto del salto
    [SerializeField] private float leapDelay = 0.5f; // Pausa antes de saltar
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

        attackCooldownTimer += Time.deltaTime;

        PerceptionUpdate(); 

        // Siempre persigue al jugador
        agent.isStopped = false;
        agent.SetDestination(player.position);

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange && attackCooldownTimer >= enemyData.AttackCooldown && CanSeePlayer)
        {
           StartCoroutine(JumpAndBite());
        }
    }

    private IEnumerator JumpAndBite()
    {
        isAttacking = true;
        // Aquí puedes poner animación de agacharse
        yield return new WaitForSeconds(leapDelay);
        audioSource.PlayOneShot(atkClip);
        // Ataque en área pequeña
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Player"));

        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(enemyData.Damage);
            }
        }

        attackCooldownTimer = 0f;
        isAttacking = false;
    }
    #region Gizmos (opcional)
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyData.DistanceToPlayer);
        LineOfSight.DrawLOSOnGizmos(transform, visionAngle, visionRange);
    }
#endif
    #endregion
}
