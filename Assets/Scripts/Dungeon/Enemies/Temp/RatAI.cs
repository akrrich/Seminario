using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Lógica de movimiento y ataque de la Rata.
/// Se apoya en un NavMeshAgent para navegar.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class RatAI : EnemyBase
{
    [Header("Movimiento errático")]
    [SerializeField] private float circleRadius = 2f;  // radio del órbita
    [SerializeField] private float directionChangeInterval = 0.4f;
    [Space(2)]
    [Header("Audio")]
    [SerializeField] private AudioClip atkClip;

    // --- Internos ---
  
    private Vector3 currentOffset;
    private float dirTimer;

    private float attackCooldownTimer;
    private bool isAttacking;
    #region Unity
    private void Start()
    {
        // --- Configurar NavMeshAgent según datos ---
        agent = GetComponent<NavMeshAgent>();
        agent.speed = enemyData.Speed;
        agent.acceleration = 30f;
        agent.angularSpeed = 720f;
        agent.stoppingDistance = enemyData.DistanceToPlayer; // 3 m
        agent.autoBraking = false;
        agent.isStopped = true;

        dirTimer = directionChangeInterval;
    }

    private void Update()
    {
        if (player == null || isAttacking) return;

        attackCooldownTimer += Time.deltaTime;

        PerceptionUpdate();

        if (canSeePlayer)
        {
            ChasePlayer();
        }
        else
        {
            ErraticMove();
        }

    }

    #endregion

    #region Movimiento

    private void ChasePlayer()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        agent.isStopped = false;
        agent.SetDestination(player.position);

        if (dist <= enemyData.DistanceToPlayer)
        {
            agent.isStopped = true;
            TryAttack();
        }
    }

    private void ErraticMove()
    {
        agent.speed = enemyData.Speed * 1.2f; 

        dirTimer -= Time.deltaTime;
        if (dirTimer <= 0f)
        {
            Vector2 rnd = Random.insideUnitCircle.normalized * circleRadius;
            currentOffset = new Vector3(rnd.x, 0f, rnd.y);

            Vector3 targetPos = player.position + currentOffset;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPos, out hit, 1.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                agent.SetDestination(player.position); // fallback
            }

            dirTimer = directionChangeInterval;
        }

        agent.isStopped = false;

        // Si está lo suficientemente cerca, intenta atacar
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= enemyData.DistanceToPlayer)
        {
            agent.isStopped = true;
            TryAttack();
        }
    }

    #endregion
    #region Ataque
    private void TryAttack()
    {
        if (attackCooldownTimer < enemyData.AttackCooldown) return;

        playerDamageable?.TakeDamage(enemyData.Damage);

        Debug.Log($"[{name}] Mordisco al jugador ({enemyData.Damage} de daño).");
        audioSource.PlayOneShot(atkClip);
        attackCooldownTimer = 0f;
        StartCoroutine(AttackDelay());
    }

    // Breve “pause” para simular la animación de mordida
    private IEnumerator AttackDelay()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.3f);  // “ventana” de animación
        isAttacking = false;
    }
    #endregion

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
