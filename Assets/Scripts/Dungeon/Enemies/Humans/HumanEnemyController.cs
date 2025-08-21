using UnityEngine;

public class HumanEnemyController : BaseEnemyController
{
    protected override void InitializeTree()
    {
        // Acciones
        ActionNode idle = new ActionNode(() => fsm.TransitionTo(EnemyStates.Idle));
        ActionNode chase = new ActionNode(() => fsm.TransitionTo(EnemyStates.Chase));
        ActionNode attack = new ActionNode(() => fsm.TransitionTo(EnemyStates.Attack));
        ActionNode dead = new ActionNode(() => fsm.TransitionTo(EnemyStates.Dead));

        // Preguntas (modularizadas)
        QuestionNode qIsDead = new QuestionNode(QuestionIsDead, dead, null);
        QuestionNode qIsInShootRange = new QuestionNode(QuestionIsInShootRange, attack, chase);
        QuestionNode qCanSeePlayer = new QuestionNode(QuestionCanSeePlayer, qIsInShootRange, idle);

        root = new QuestionNode(QuestionIsDead, dead, qCanSeePlayer);
    }

    public override void Attack()
    {
        base.Attack();
        // Ejemplo de lógica extendida
        Debug.Log($"{gameObject.name} lanza un proyectil o habilidad especial");
    }

    // ==== Preguntas del árbol para humanos ====

    protected override bool QuestionIsDead()
    {
        return model.CurrentHP <= 0;
    }

    protected override bool QuestionCanSeePlayer()
    {
        return model.CanSeePlayer();
    }

    private bool QuestionIsInShootRange()
    {
        if (model.Player == null) return false;

        float distance = Vector3.Distance(transform.position, model.Player.transform.position);
        return distance <= 7f; // Rango extendido para humanos con armas a distancia
    }
}
