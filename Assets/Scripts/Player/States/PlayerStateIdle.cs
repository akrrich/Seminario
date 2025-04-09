using UnityEngine;

public class PlayerStateIdle<T> : State<T>
{
    private PlayerModel playerModel;

    private T inputToWalk;
    private T inputToJump;
    private T inputToCook;


    public PlayerStateIdle(T inputToWalk, T inputToJump, T inputToCook, PlayerModel playerModel)
    {
        this.inputToWalk = inputToWalk;
        this.inputToJump = inputToJump;
        this.inputToCook = inputToCook;
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

        if (playerModel.GetMoveAxis() != Vector2.zero)
        {
            Fsm.TransitionTo(inputToWalk);
        }

        if (PlayerInputs.Instance.Jump() && playerModel.IsGrounded)
        {
            Fsm.TransitionTo(inputToJump);
        }

        if (PlayerInputs.Instance.Cook() && playerModel.IsCollidingOven && playerModel.IsLookingAtOven())
        {
            Fsm.TransitionTo(inputToCook);
        }
    }
}
