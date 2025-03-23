using UnityEngine;

public class PlayerStateIdle<T> : State<T>
{
    private PlayerModel playerModel;

    private T inputToWalk;
    private T inputToJump;


    public PlayerStateIdle(T inputToWalk, T inputToJump, PlayerModel playerModel)
    {
        this.inputToWalk = inputToWalk;
        this.inputToJump = inputToJump;
        this.playerModel = playerModel;
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

        if (Input.GetKeyDown(KeyCode.Space) && playerModel.IsGrounded)
        {
            Fsm.TransitionTo(inputToJump);
        }
    }
}
