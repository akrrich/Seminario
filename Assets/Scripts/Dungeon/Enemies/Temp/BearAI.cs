using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class BearAI : EnemyBase
{
    [Header("Oso")]
    [SerializeField] private float attackRange = 4f; // Distancia para atacar
    [SerializeField] private float attackArea = 5f;  // Área del ataque frontal
    [Space(2)]
    [Header("Audio")]
    [SerializeField] private AudioClip atkClip;
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

        // Movimiento lineal hacia el jugador
        agent.isStopped = false;
        agent.SetDestination(player.position);

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange && attackCooldownTimer >= enemyData.AttackCooldown)
        {
            agent.isStopped = true;
            StartCoroutine(BearClawAttack());
        }
    }

    private IEnumerator BearClawAttack()
    {
        isAttacking = true;
        // Animación de ataque
        yield return new WaitForSeconds(0.4f); // simula animación
        audioSource.PlayOneShot(atkClip);
        // Ataque en área grande frente a él
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 2f, attackArea);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<IDamageable>()?.TakeDamage(enemyData.Damage);
            }
        }
        attackCooldownTimer = 0f;
        isAttacking = false;
    }
}
