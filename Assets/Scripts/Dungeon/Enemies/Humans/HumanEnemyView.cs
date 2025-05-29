using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanEnemyView : BaseEnemyView
{
    public override void PlayAttackAnimation()
    {
        base.PlayAttackAnimation();
        // Ejemplo: mostrar animación de disparo mágico
        Debug.Log("HumanEnemyView: ataque especial con hechizo o flecha.");
    }

    public override void PlayDeathAnimation()
    {
        base.PlayDeathAnimation();
        // Podrías instanciar partículas o sonidos humanos
        Debug.Log("HumanEnemyView: animación de muerte personalizada.");
    }
}
