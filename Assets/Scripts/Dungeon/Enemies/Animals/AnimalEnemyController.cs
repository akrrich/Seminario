using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalEnemyController : BaseEnemyController
{
    private AnimalEnemyModel animalModel;

    protected override void GetComponents()
    {
        base.GetComponents();
        animalModel = GetComponent<AnimalEnemyModel>();
    }

    protected override void InitializeTree()
    {
        ActionNode idle = new ActionNode(() => fsm.TransitionTo(EnemyStates.Idle));
        ActionNode chase = new ActionNode(() => fsm.TransitionTo(EnemyStates.Chase));
        ActionNode attack = new ActionNode(() => fsm.TransitionTo(EnemyStates.Attack));
        ActionNode dead = new ActionNode(() => fsm.TransitionTo(EnemyStates.Dead));
      
        QuestionNode qIsDead = new QuestionNode(QuestionIsDead, dead, null);
        QuestionNode qIsInAttackRange = new QuestionNode(QuestionIsInAttackRange, attack, qIsDead);
        QuestionNode qCanSeePlayer = new QuestionNode(QuestionCanSeePlayer, qIsInAttackRange, idle);

        root = new QuestionNode(QuestionIsDead, dead, qCanSeePlayer);
    }

    public override void Attack()
    {
        base.Attack();
        // Sonido brutal, animación de mordida, etc.
        Debug.Log("AnimalEnemyController: ataque cuerpo a cuerpo.");
    }

    // ==== Preguntas ====

    protected override bool QuestionIsDead() => animalModel.CurrentHP <= 0;

    protected override bool QuestionCanSeePlayer() => animalModel.CanSeePlayer();

    protected override bool QuestionIsInAttackRange()
    {
        if (animalModel.Player == null) return false;
        return Vector3.Distance(transform.position, animalModel.Player.transform.position) <= 2f;
    }

  
}