
using UnityEngine;
using UnityEngine.AI;

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
    protected int currentHP;
    protected bool isDead = false;

    protected virtual void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError($"[{name}] Falta asignar EnemyData.");
            enabled = false;
            return;
        }
        currentHP = enemyData.HP;
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
}
