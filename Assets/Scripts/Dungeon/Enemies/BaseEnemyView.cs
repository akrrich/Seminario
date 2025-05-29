using UnityEngine;

public class BaseEnemyView : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public virtual void PlayMoveAnimation(bool isMoving) => animator?.SetBool("IsMoving", isMoving);
    public virtual void PlayAttackAnimation() => animator?.SetTrigger("Attack");
    public virtual void PlayDeathAnimation() => animator?.SetTrigger("Die");
    public virtual void PlayCastAnimation() => animator?.SetTrigger("Cast");
    public virtual void PlayFleeAnimation() => animator?.SetTrigger("Flee");
}
