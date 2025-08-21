
using UnityEngine;

public class AnimalEnemyView : BaseEnemyView
{
    public override void PlayAttackAnimation()
    {
        base.PlayAttackAnimation();
        // Sonido de mordida o animación de embestida
        Debug.Log("AnimalEnemyView: animación de ataque físico salvaje.");
    }

    public override void PlayDeathAnimation()
    {
        base.PlayDeathAnimation();
        // Rugido, sacudida o colapso
        Debug.Log("AnimalEnemyView: animación de muerte brutal.");
    }
}
