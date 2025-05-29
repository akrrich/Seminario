
using UnityEngine;

public class AnimalEnemyView : BaseEnemyView
{
    public override void PlayAttackAnimation()
    {
        base.PlayAttackAnimation();
        // Sonido de mordida o animaci�n de embestida
        Debug.Log("AnimalEnemyView: animaci�n de ataque f�sico salvaje.");
    }

    public override void PlayDeathAnimation()
    {
        base.PlayDeathAnimation();
        // Rugido, sacudida o colapso
        Debug.Log("AnimalEnemyView: animaci�n de muerte brutal.");
    }
}
