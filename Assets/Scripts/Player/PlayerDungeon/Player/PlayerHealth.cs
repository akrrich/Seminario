using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Stats")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float healAmount = 10f;
    [SerializeField] private float healInterval = 30f;

    //---Private Fields---
    private float currentHP;
    private bool isDead = false;
    private bool invulnerable = false;

    public float CurrentHP => currentHP;
    public bool IsDead => isDead;

    //---Events---
    public event Action<float, float> OnHealthChanged;
    public event Action OnPlayerDied;

    private void Awake()
    {
        currentHP = maxHP;
        OnHealthChanged?.Invoke(currentHP, maxHP);
        StartCoroutine(AutoHeal());
    }

    public void TakeDamage(int amount)
    {
        if (invulnerable || isDead) return;

        currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);
        OnHealthChanged?.Invoke(currentHP, maxHP);
        CombatFeedbackManager.Instance.PlayRandomDamageSound(transform.position);
        CombatFeedbackManager.Instance.ShakeCamera(0.15f, 0.25f, 5f);

        if (currentHP <= 0 && !isDead)
        {
            currentHP = 0;
            isDead = true;
            OnPlayerDied?.Invoke();
        }
    }

    private IEnumerator AutoHeal()
    {
        while (true)
        {
            yield return new WaitForSeconds(healInterval);

            if (!isDead && currentHP < maxHP)
            {
                currentHP = Mathf.Clamp(currentHP + healAmount, 0, maxHP);
                OnHealthChanged?.Invoke(currentHP, maxHP);
            }
        }
    }

    public void SetInvulnerable(bool value) => invulnerable = value;

    public void Revive()
    {
        currentHP = maxHP;
        isDead = false;
        SetInvulnerable(false);
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }
}
