using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDungeonView : MonoBehaviour
{
    private Animator animator;
    private PlayerDungeonModel model;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        model = GetComponent<PlayerDungeonModel>();
    }

    private void OnEnable()
    {
        model.OnHealthChanged += HandleHealthChanged;
        model.OnPlayerDied += HandleDeath;
    }

    private void OnDisable()
    {
        model.OnHealthChanged -= HandleHealthChanged;
        model.OnPlayerDied -= HandleDeath;
    }

    private void HandleHealthChanged(float current, float max)
    {
        // Acá disparamos el evento global para el HUD
        PlayerDungeonHUD.OnHealthChanged?.Invoke(current, max);
    }

    private void HandleDeath()
    {
        animator.SetTrigger("Die");
        PlayerDungeonHUD.OnPlayerDeath?.Invoke();
    }

    // Animaciones
    public void PlayWalkAnimation(bool walking) => animator?.SetBool("IsWalking", walking);
    public void PlayAttackAnimation() => animator?.SetTrigger("Attack");
    public void PlayDashAnimation() => animator?.SetTrigger("Dash");
    public void OnAttackFrame() => GetComponent<AttackHitbox>()?.TriggerHit();
}
