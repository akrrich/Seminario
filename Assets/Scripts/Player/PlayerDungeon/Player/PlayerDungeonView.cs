using UnityEngine;

public class PlayerDungeonView : MonoBehaviour
{
    private Animator animator;
    private PlayerHealth health;
    private PlayerDungeonModel model;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        model = GetComponent<PlayerDungeonModel>();
        if (health == null)
            Debug.LogError("PlayerHealth no encontrado en PlayerDungeonView.");
        health = GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        health.OnHealthChanged += HandleHealthChanged;
        health.OnPlayerDied += HandleDeath;
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= HandleHealthChanged;
        health.OnPlayerDied -= HandleDeath;
    }

    private void HandleHealthChanged(float current, float max)
    {
        PlayerDungeonHUD.OnHealthChanged?.Invoke(current, max);
    }

    private void HandleDeath()
    {
        animator.SetTrigger("Die");
        PlayerDungeonHUD.OnPlayerDeath?.Invoke();
        DungeonManager.Instance?.OnPlayerDeath();
    }

    // Animaciones
    public void PlayAttackAnimation() => animator?.SetTrigger("Attack");
    public void OnAttackFrame() => GetComponent<AttackHitbox>()?.TriggerHit();

}
