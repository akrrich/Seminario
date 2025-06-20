using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour,IDamageable
{
    public EnemyData enemyData;
    private int currentHP;
    private NavMeshAgent agent;
    private RatAI aiController;
    private bool isDead = false;

    void Awake()
    {
        currentHP = enemyData.HP;
        agent = GetComponent<NavMeshAgent>();
        aiController = GetComponent<RatAI>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        Debug.Log($"Rata recibió {amount} de daño. Vida restante: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("¡La rata ha muerto!");
        agent.isStopped = true;
        if (aiController != null) aiController.enabled = false;

        Destroy(gameObject, 1.5f); // tiempo antes de destruir, por si hay animación
    }
}
