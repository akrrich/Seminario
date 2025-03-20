using UnityEngine;

public class PlayerStateWalk<T> : State<T>
{
    private T inputToIdle;


    public PlayerStateWalk(T inputToIdle)
    {
        this.inputToIdle = inputToIdle;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Walk");
    }

    public override void Execute()
    {
        base.Execute();

        Vector3 direction = new Vector3(PlayerModel.GetMove().x, 0, PlayerModel.GetMove().y);

        if (direction == Vector3.zero)
        {
            Fsm.TransitionTo(inputToIdle);
        }
    }
}
