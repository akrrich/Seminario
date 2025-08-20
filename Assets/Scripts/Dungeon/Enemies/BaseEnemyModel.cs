using UnityEngine;
using UnityEngine.AI;

public class BaseEnemyModel : MonoBehaviour, IDamageable
{
    [Header("Configuración del Enemigo")]
    [SerializeField] protected EnemyData baseStats;

    [Header("Estado Actual")]
    [SerializeField] private bool isElite = false;
    [SerializeField] private int currentLayer = 1;



    protected NavMeshAgent agent;
    protected PlayerMovement playerRef;
    protected int currentHP;

    public EnemyData BaseStats => baseStats;
    public virtual int Damage => Mathf.CeilToInt(GetScaledStat(baseStats.Damage));
    public virtual float Speed => GetScaledStat(baseStats.Speed);
    public virtual float AttackCooldown => GetScaledStat(baseStats.AttackCooldown);
    public virtual int MaxHP => Mathf.CeilToInt(GetScaledStat(baseStats.HP));
    public int CurrentHP => currentHP;

    public PlayerMovement Player => playerRef;
    public NavMeshAgent Agent => agent;
    public bool IsElite => isElite;
    protected virtual void Update()
    {

    }
    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        playerRef = FindFirstObjectByType<PlayerMovement>();
        currentHP = MaxHP;

        agent.speed = Speed;
    }

    public virtual void Initialize(int layer, bool elite)
    {
        currentLayer = layer;
        isElite = elite;
        currentHP = MaxHP;
        agent.speed = Speed;
    }

    protected float GetScaledStat(float baseValue)
    {
        float scale = 1f;
        if (currentLayer >= 3 && currentLayer <= 4) scale = 1.1f;
        else if (currentLayer >= 5 && currentLayer <= 6) scale = 1.15f;
        else if (currentLayer >= 7) scale = 1.2f;

        if (isElite) scale += 0.25f;
        return baseValue * scale;
    }

    public virtual void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
            Die();
    }

    protected virtual void Die()
    {
        DropLoot();
        Stop();
        gameObject.SetActive(false);
    }

    protected virtual void DropLoot()
    {
        // Virtual. Cada modelo derivado dropea cosas distintas.
    }

    public void MoveTo(Vector3 destination)
    {
        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    public void Stop()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
    }

    public void LookAt(Vector3 position)
    {
        Vector3 dir = position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    public bool CanSeePlayer()
    {
        return LineOfSight.LOS(transform, playerRef.transform, 10f, 60f, LayerMask.GetMask("Obstacles"));
    }
}
