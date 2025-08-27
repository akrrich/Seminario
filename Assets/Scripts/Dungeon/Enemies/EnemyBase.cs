
using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Componente principal del enemigo. Gestiona HP, muerte
/// y delega el comportamiento a RatAI.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyBase : MonoBehaviour,IDamageable
{
    [Header("Data")]
    public EnemyData enemyData;
    [SerializeField] private string id;
    public string Id => id;

    [Header("LoS")]
    [SerializeField] protected float visionRange = 8f;
    [SerializeField] protected float visionAngle = 100f;
    [SerializeField] protected LayerMask obstacleMask;
    [SerializeField] protected float chaseSpeedMultiplier = 1.5f;
    [SerializeField] protected float loseSightDelay = 1.5f;

    [Header("Decals")]
    [SerializeField] private GameObject bloodDecalPrefab;
    [SerializeField] private float decalYOffset = 0.01f; // Para evitar z-fighting
   
    [Header("Runtime")]
    public int CurrentHP { get; private set; }
    public bool IsDead { get; private set; }
    //---References---
    protected DropHandler dropHandler;
    protected DamageFlash damageFlash;
    protected NavMeshAgent agent;
    protected IDamageable playerDamageable;
    protected Transform player;
    protected AudioSource audioSource;

    protected bool canSeePlayer = false;
    protected float loseSightTimer = 0f;

    public bool CanSeePlayer => canSeePlayer;

    public event Action<EnemyBase> OnDeath;

    protected virtual void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError($"[{name}] Missing EnemyData.");
            enabled = false;
            return;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerDamageable = playerObj.GetComponent<IDamageable>();
        }

        agent = GetComponent<NavMeshAgent>();
        damageFlash = GetComponent<DamageFlash>();
        dropHandler = GetComponent<DropHandler>();
        audioSource = GetComponent<AudioSource>();

        CurrentHP = enemyData.HP;
        IsDead = false;
    }
    public virtual void TakeDamage(int amount)
    {
        if (IsDead) return;

        CurrentHP -= amount;
        damageFlash?.TriggerFlash();
        SpawnBloodDecal();

        if (CurrentHP <= 0)
            Die();
    }
    public void SetScaledStats(float hpMul, float dmgMul, float speedMul)
    {
        enemyData = Instantiate(enemyData);

        enemyData.HP = Mathf.RoundToInt(enemyData.HP * (1 + hpMul));
        enemyData.Damage = Mathf.RoundToInt(enemyData.Damage * (1 + dmgMul));
        enemyData.Speed *= (1 + speedMul);

        CurrentHP = enemyData.HP;
        agent.speed = enemyData.Speed;
    }
    protected virtual void Die()
    {
        if (IsDead) return;

        IsDead = true;
        agent.isStopped = true;

        dropHandler?.DropLoot();
        SpawnBloodDecal();

        OnDeath?.Invoke(this);

        Destroy(gameObject, 1.5f);
    }

    protected void PerceptionUpdate()
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
        }
    }
    private void SpawnBloodDecal()
    {
        if (bloodDecalPrefab == null) return;

        Vector3 origin = transform.position + Vector3.up * 1f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 10f, LayerMask.GetMask("whatIsGround")))
        {
            Vector3 spawnPos = hit.point + Vector3.up * decalYOffset;
            Quaternion rot = Quaternion.Euler(90f, UnityEngine.Random.Range(0f, 360f), 0f);
            GameObject decal = Instantiate(bloodDecalPrefab, spawnPos, rot);
            Destroy(decal, 5f);
        }
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
