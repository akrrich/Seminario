
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

/// <summary>
/// Componente principal del enemigo. Gestiona HP, muerte
/// y delega el comportamiento a RatAI.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyBase : MonoBehaviour,IDamageable
{
    [Header("References")]
    public EnemyData enemyData;
    protected DropHandler dropHandler;
    protected DamageFlash damageFlash;
    protected NavMeshAgent agent;
    protected IDamageable playerDamageable;

    [Space(2)]
    [Header("LoS")]
    [SerializeField] protected float visionRange = 8f;
    [SerializeField] protected float visionAngle = 100f;
    [SerializeField] protected LayerMask obstacleMask;
    [SerializeField] protected float chaseSpeedMultiplier = 1.5f;
    [SerializeField] protected float loseSightDelay = 1.5f;

    protected Transform player;
    protected bool canSeePlayer = false;
    protected float loseSightTimer = 0f;

    [Header("Health (runtime)")]
    public int currentHP;
    protected bool isDead = false;
    protected AudioSource audioSource;

    [Header("Room Tracking")]
    public RoomData roomData;

    [Header("Spawner ID")]
    [SerializeField] private string id;
    public string Id => id;
    public bool CanSeePlayer => canSeePlayer;

    protected virtual void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError($"[{name}] Falta asignar EnemyData.");
            enabled = false;
            return;
        }
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerDamageable = playerObj.GetComponent<IDamageable>();
        }
        else
        {
            Debug.LogWarning($"[{name}] No se encontró ningún GameObject con tag 'Player'.");
            enabled = false;
        }

        currentHP = enemyData.HP;
        audioSource = GetComponent<AudioSource>();
        dropHandler = GetComponent<DropHandler>();
        agent = GetComponent<NavMeshAgent>();
        damageFlash = GetComponent<DamageFlash>();
        if (dropHandler == null)
        {
            Debug.LogWarning($"[{name}] No se encontró DropHandler en el objeto.");
        }
    }
    protected virtual void Die()
    {
        isDead = true;
        agent.isStopped = true;

        dropHandler?.DropLoot();

        if (roomData != null)
        {
            roomData.NotifyEnemyDied(this.gameObject);
        }

        Destroy(gameObject, 1.5f);
    }

    protected virtual void PerceptionUpdate()
    {
        if (player == null) return;

        bool seesPlayer = LineOfSight.LOS(transform, player, visionRange, visionAngle, obstacleMask);

        if (seesPlayer)
        {
            loseSightTimer = 0f;

            if (!canSeePlayer)
            {
                canSeePlayer = true;
                agent.speed = enemyData.Speed * chaseSpeedMultiplier;
            }
        }
        else
        {
            loseSightTimer += Time.deltaTime;

            if (canSeePlayer && loseSightTimer >= loseSightDelay)
            {
                canSeePlayer = false;
                agent.speed = enemyData.Speed;
                loseSightTimer = 0f;
            }

            if (!canSeePlayer)
            {
                agent.speed = enemyData.Speed;
            }
        }
    }

    public virtual void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHP -= amount;
        damageFlash?.TriggerFlash();

        if (currentHP <= 0) Die();
    }

    public void SetScaledStats(float hpMul, float dmgMul, float speedMul)
    {
        enemyData = Instantiate(enemyData); // Clona los datos base

        enemyData.HP = Mathf.RoundToInt(enemyData.HP * (1 + hpMul));
        enemyData.Damage = Mathf.RoundToInt(enemyData.Damage * (1 + dmgMul));
        enemyData.Speed *= Mathf.RoundToInt(1 + speedMul);

        currentHP = enemyData.HP;
        agent.speed = enemyData.Speed;
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
