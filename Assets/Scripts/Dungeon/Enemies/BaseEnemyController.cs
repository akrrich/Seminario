using UnityEngine;

public abstract class BaseEnemyController : MonoBehaviour, IAttackable
{
    protected BaseEnemyModel model;
    protected BaseEnemyView view;

    protected FSM<EnemyStates> fsm = new FSM<EnemyStates>();
    protected ITreeNode root;

    // Estados individuales
    protected EnemyStateIdle<EnemyStates> esIdle;
    protected EnemyStateChase<EnemyStates> esChase;
    protected EnemyStateAttack<EnemyStates> esAttack;
    protected EnemyStateDead<EnemyStates> esDead;

    protected virtual void Awake()
    {
        GetComponents();
    }

    protected virtual void Start()
    {
        InitializeFSM();
        InitializeTree();
    }

    protected virtual void Update()
    {
        fsm.OnExecute();
        root.Execute();
    }

    protected virtual void GetComponents()
    {
        model = GetComponent<BaseEnemyModel>();
        view = GetComponent<BaseEnemyView>();
    }

    /// <summary>
    /// Este método puede ser sobrescrito por subclases si cambian el comportamiento de ataque (por ejemplo, proyectiles).
    /// </summary>
    public virtual void Attack()
    {
        view.PlayAttackAnimation();
        Debug.Log($"{gameObject.name} attacks player for {model.Damage} damage");
        // Aquí podrías aplicar daño al jugador si hay acceso directo.
    }

    /// <summary>
    /// Los estados base pueden ser compartidos. Subclases pueden sobrescribir si cambian FSM.
    /// </summary>
    protected virtual void InitializeFSM()
    {
        esIdle = new EnemyStateIdle<EnemyStates>(model, view);
        esChase = new EnemyStateChase<EnemyStates>(model, view);
        esAttack = new EnemyStateAttack<EnemyStates>(model, view, this);
        esDead = new EnemyStateDead<EnemyStates>(model, view);

        esIdle.AddTransition(EnemyStates.Chase, esChase);
        esChase.AddTransition(EnemyStates.Attack, esAttack);
        esAttack.AddTransition(EnemyStates.Chase, esChase);
        esAttack.AddTransition(EnemyStates.Dead, esDead);

        fsm.SetInit(esIdle);
    }

    /// <summary>
    /// El árbol por defecto: Idle ? Chase ? Attack ? Dead.
    /// Subclases pueden extenderlo (por ejemplo, clérigos que curan).
    /// </summary>
    protected virtual void InitializeTree()
    {
        ActionNode idle = new ActionNode(() => fsm.TransitionTo(EnemyStates.Idle));
        ActionNode chase = new ActionNode(() => fsm.TransitionTo(EnemyStates.Chase));
        ActionNode attack = new ActionNode(() => fsm.TransitionTo(EnemyStates.Attack));
        ActionNode dead = new ActionNode(() => fsm.TransitionTo(EnemyStates.Dead));

        QuestionNode qIsDead = new QuestionNode(QuestionIsDead, dead, null);
        QuestionNode qIsInAttackRange = new QuestionNode(QuestionIsInAttackRange, attack, chase);
        QuestionNode qCanSeePlayer = new QuestionNode(QuestionCanSeePlayer, qIsInAttackRange, idle);

        root = new QuestionNode(QuestionIsDead, dead, qCanSeePlayer);
    }

    // ==== Preguntas base para el árbol de comportamiento ====

    protected virtual bool QuestionIsDead()
    {
        return model.CurrentHP <= 0;
    }

    protected virtual bool QuestionCanSeePlayer()
    {
        return model.CanSeePlayer();
    }

    protected virtual bool QuestionIsInAttackRange()
    {
        if (model.Player == null) return false;
        return Vector3.Distance(transform.position, model.Player.transform.position) < 2f;
    }
}
