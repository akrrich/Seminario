using System.Collections.Generic;

public class State<T> : IState<T>
{
    private FSM<T> fsm;
    private Dictionary<T, IState<T>> transitions = new Dictionary<T, IState<T>>();

    public FSM<T> Fsm { get => fsm; set => fsm = value; }


    public virtual void Initialize(params object[] p) { }

    public virtual void Enter() { }

    public virtual void Execute() { }

    public virtual void Exit() { }

    public IState<T> GetTransition(T input)
    {
        if (!transitions.ContainsKey(input))
        {
            return null;
        }

        return transitions[input];
    }

    public void AddTransition(T input, IState<T> state)
    {
        transitions[input] = state;
    }

    public void RemoveTransition(T input)
    {
        if (transitions.ContainsKey(input))
        {
            transitions.Remove(input);
        }
    }

    public void RemoveTransition(IState<T> state)
    {
        foreach (var item in transitions)
        {
            if (item.Value == state)
            {
                transitions.Remove(item.Key);
                break;
            }
        }
    }
}
