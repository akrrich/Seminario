using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class WolfAI : EnemyBase
{
    [Header("Lobo")]
    [SerializeField] private float attackRange = 5f; // �rea de impacto del salto
    [SerializeField] private float leapDelay = 0.5f; // Pausa antes de saltar

    private Transform player;
    private float attackCooldownTimer = 0f;
    private bool isAttacking;

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.speed = enemyData.Speed;
    }

    private void Update()
    {
        if (isDead || isAttacking) return;

        attackCooldownTimer += Time.deltaTime;

        // Movimiento directo hacia el jugador
        agent.isStopped = false;
        agent.SetDestination(player.position);

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            agent.isStopped = true;
            if (attackCooldownTimer >= enemyData.AttackCooldown)
            {
                StartCoroutine(JumpAndBite());
            }
        }
    }

    private IEnumerator JumpAndBite()
    {
        isAttacking = true;
        // Aqu� puedes poner animaci�n de agacharse
        yield return new WaitForSeconds(leapDelay);
        // Aqu� animaci�n de salto (agrega Rigidbody o movimiento r�pido)
        // Ataque en �rea peque�a
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Player"));

        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(enemyData.Damage);
            }
        }

        attackCooldownTimer = 0f;
        isAttacking = false;
    }
}
