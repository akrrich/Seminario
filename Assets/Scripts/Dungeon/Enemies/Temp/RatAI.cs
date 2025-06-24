using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Lógica de movimiento y ataque de la Rata.
/// Se apoya en un NavMeshAgent para navegar.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class RatAI : MonoBehaviour
{
    [Header("Stats / Data")]
    public EnemyData enemyData;

    [Header("Movimiento errático")]
    [SerializeField] private float circleRadius = 2f;  // radio del órbita
    [SerializeField] private float directionChangeInterval = 0.4f;

    // --- Internos ---
    private Transform player;
    private IDamageable playerDamageable;
    private NavMeshAgent agent;
    private Vector3 currentOffset;
    private float dirTimer;

    private float attackCooldownTimer;
    private bool isAttacking;

    #region Unity
    private void Start()
    {
        // === Validaciones ===
        if (enemyData == null)
        {
            Debug.LogError($"[{name}] Falta asignar EnemyData.");
            enabled = false;
            return;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("No se encontró ningún GameObject con tag 'Player'.");
            enabled = false;
            return;
        }

        player = playerObj.transform;
        playerDamageable = playerObj.GetComponent<IDamageable>();

        // --- Configurar NavMeshAgent según datos ---
        agent = GetComponent<NavMeshAgent>();
        agent.speed = enemyData.Speed;
        agent.acceleration = 30f;
        agent.angularSpeed = 720f;
        agent.stoppingDistance = enemyData.DistanceToPlayer; // 3 m
        agent.autoBraking = false;

        dirTimer = directionChangeInterval;
    }

    private void Update()
    {
        if (player == null || isAttacking) return;

        attackCooldownTimer += Time.deltaTime;

        // ---------- Movimiento errático ----------
        dirTimer -= Time.deltaTime;
        if (dirTimer <= 0f)
        {
            Vector2 rnd = Random.insideUnitCircle.normalized * circleRadius;
            currentOffset = new Vector3(rnd.x, 0f, rnd.y);
            dirTimer = directionChangeInterval;
        }

        Vector3 targetPos = player.position + currentOffset;
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > enemyData.DistanceToPlayer)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPos);
        }
        else
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
    }
#endif
    #endregion
}
