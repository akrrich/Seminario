
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Componente principal del enemigo. Gestiona HP, muerte
/// y delega el comportamiento a RatAI.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour,IDamageable
{
    [Header("References")]
    [Tooltip("ScriptableObject con las stats de la rata")]
    public EnemyData enemyData;

    private DamageFlash damageFlash;
    private NavMeshAgent agent;
    private RatAI aiController;

    [Header("Health (runtime)")]
    private int currentHP;
    private bool isDead = false;


    private void Awake()
    {
        
        if (enemyData == null)
        {
            Debug.LogError($"[{name}] -> Falta asignar EnemyData.");
            enabled = false;
            return;
        }

        currentHP = enemyData.HP;          
        agent = GetComponent<NavMeshAgent>();
        damageFlash = GetComponent<DamageFlash>();
        aiController = GetComponent<RatAI>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        Debug.Log($"[{name}] Rata recibió {amount} de daño. Vida restante: {currentHP}");
        damageFlash?.TriggerFlash();

        if (currentHP <= 0) Die();
    }


    private void Die()
    {
        isDead = true;
        Debug.Log($"[{name}] ¡La rata ha muerto!");

        agent.isStopped = true;
        if (aiController != null) aiController.enabled = false;

        // Aca van las animaciones, partículas, etc.
        Destroy(gameObject, 1.5f);   // margen para anim / VFX
    }
}
