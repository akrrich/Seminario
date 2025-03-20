public class FSM<T>
{
    private IState<T> currentState;


    public void SetInit(IState<T> current)
    {
        current.Fsm = this;
        currentState = current;
        currentState.Enter();
    }

    public void OnExecute()
    {
        if (currentState != null)
        {
            currentState.Execute();
        }
    }

    public void TransitionTo(T input)
    {
        IState<T> newState = currentState.GetTransition(input);

        if (newState == null)
        {
            return;
        }

        newState.Fsm = this;
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
