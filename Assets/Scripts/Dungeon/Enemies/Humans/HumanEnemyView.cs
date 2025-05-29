using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanEnemyView : BaseEnemyView
{
    public override void PlayAttackAnimation()
    {
        base.PlayAttackAnimation();
        // Ejemplo: mostrar animaci�n de disparo m�gico
        Debug.Log("HumanEnemyView: ataque especial con hechizo o flecha.");
    }

    public override void PlayDeathAnimation()
    {
        base.PlayDeathAnimation();
        // Podr�as instanciar part�culas o sonidos humanos
        Debug.Log("HumanEnemyView: animaci�n de muerte personalizada.");
    }
}
