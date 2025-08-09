
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    private PlayerDungeonModel model;
    private PlayerDungeonView view;

    [SerializeField] private AttackHitbox attackHitbox;
    [SerializeField] private WeaponController weaponController;

    private float lastAttackTime = -999f;

    private void Awake()
    {
        GetComponents();      
    }

    public void TryAttack()
    {
        if (Time.time < lastAttackTime + model.AttackCooldown) return;

        lastAttackTime = Time.time;
        
        view?.PlayAttackAnimation();
        weaponController?.PerformAttack();
        PerformHit();
    }
    public void PerformHit()
    {
        attackHitbox.TriggerHit();
    }
    private void GetComponents()
    {
        model = GetComponent<PlayerDungeonModel>();
        view = GetComponent<PlayerDungeonView>();
        attackHitbox = GetComponent<AttackHitbox>();
        weaponController = GetComponentInChildren<WeaponController>();
    }
}
