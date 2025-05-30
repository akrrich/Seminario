using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    private PlayerDungeonModel model;
    private PlayerDungeonView view;

    private float lastAttackTime = -999f;
    private float attackCooldown = 1f;

    [SerializeField] private AttackHitbox attackHitbox;

    private void Awake()
    {
        model = GetComponent<PlayerDungeonModel>();
        view = GetComponent<PlayerDungeonView>();
        attackHitbox = GetComponent<AttackHitbox>();
    }

    public void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        view.PlayAttackAnimation();
        PerformHit();
    }
    public void PerformHit()
    {
        attackHitbox.TriggerHit();
    }
}
