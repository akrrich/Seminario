using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDungeonView : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void PlayWalkAnimation(bool walking)
    {
        animator?.SetBool("IsWalking", walking);
    }

    public void PlayAttackAnimation()
    {
        animator?.SetTrigger("Attack");
    }

    public void PlayDashAnimation()
    {
        animator?.SetTrigger("Dash");
    }

    public void PlayDeathAnimation()
    {
        animator?.SetTrigger("Die");
    }
    public void OnAttackFrame()
    {
        GetComponent<AttackHitbox>()?.TriggerHit();
    }
}
