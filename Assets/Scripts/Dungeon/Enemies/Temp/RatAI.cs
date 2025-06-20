using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class RatAI : MonoBehaviour
{
    public EnemyData enemyData;

    private Transform player;
    private IDamageable playerDamageable;

    private NavMeshAgent agent;
    private float attackCooldownTimer = 0f;
    private bool isAttacking = false;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerDamageable = player.GetComponent<IDamageable>();
        }
        else
        {
            Debug.LogWarning("No se encontró un GameObject con la tag 'Player'");
        }

        agent = GetComponent<NavMeshAgent>();
        agent.speed = enemyData.Speed;
        agent.stoppingDistance = 0f;
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        agent.SetDestination(player.position);
        attackCooldownTimer += Time.deltaTime;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player") || isAttacking) return;

        if (attackCooldownTimer >= enemyData.AttackCooldown)
        {
            if (playerDamageable != null)
            {
                playerDamageable.TakeDamage(enemyData.Damage);
                Debug.Log("Rata dañó al jugador por colisión.");
            }

            attackCooldownTimer = 0f;
            StartCoroutine(AttackDelay());
        }
    }

    private System.Collections.IEnumerator AttackDelay()
    {
        isAttacking = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(0.3f);

        agent.isStopped = false;
        isAttacking = false;
    }
}