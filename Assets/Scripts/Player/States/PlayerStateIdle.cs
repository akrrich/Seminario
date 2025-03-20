using UnityEngine;

public class PlayerStateIdle<T> : State<T>
{
    private T inputToWalk;
    private T inputToJump;


    public PlayerStateIdle(T inputToWalk, T inputToJump)
    {
        this.inputToWalk = inputToWalk;
        this.inputToJump = inputToJump;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Idle");
    }

    public override void Execute()
    {
        base.Execute();

        if (PlayerModel.GetMove() != Vector2.zero)
        {
            Fsm.TransitionTo(inputToWalk);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fsm.TransitionTo(inputToJump);
        }
    }
}
