
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

    protected DamageFlash damageFlash;
    protected NavMeshAgent agent;

    [Header("Health (runtime)")]
    public int currentHP;
    protected bool isDead = false;
    protected AudioSource audioSource;

    [Header("Spawner ID")]
    [SerializeField] private string id;
    public string Id => id;

    protected virtual void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError($"[{name}] Falta asignar EnemyData.");
            enabled = false;
            return;
        }
        currentHP = enemyData.HP;
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        damageFlash = GetComponent<DamageFlash>();
    }

    public virtual void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHP -= amount;
        damageFlash?.TriggerFlash();

        if (currentHP <= 0) Die();
    }

    protected virtual void Die()
    {
        isDead = true;
        agent.isStopped = true;
        // Puedes poner animación o efectos aquí
        Destroy(gameObject, 1.5f);
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

}
