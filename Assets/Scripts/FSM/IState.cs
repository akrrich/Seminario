public interface IState<T>
{
    public FSM<T> Fsm { get; set; }
    void Initialize(params object[] p);
    void Enter();
    void Execute();
    void Exit();
    IState<T> GetTransition(T input);
    void AddTransition(T input, IState<T> state);
    void RemoveTransition(T input);
    void RemoveTransition(IState<T> state);
}
