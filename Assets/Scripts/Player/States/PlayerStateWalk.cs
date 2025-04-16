using UnityEngine;

public class PlayerStateWalk<T> : State<T>
{
    private PlayerModel playerModel;

    private T inputToIdle;
    private T inputToRun;
    private T inputToJump;
    private T inputToCook;


    public PlayerStateWalk(T inputToIdle, T inputToRun, T inputToJump, T inputToCook, PlayerModel playerModel)
    {
        this.inputToIdle = inputToIdle;
        this.inputToRun = inputToRun;
        this.inputToJump = inputToJump;
        this.inputToCook = inputToCook;
        this.playerModel = playerModel;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Walk");

        playerModel.Speed = playerModel.WalkSpeed;
    }

    public override void Execute()
    {
        base.Execute();

        if (playerModel.GetMoveAxis() == Vector2.zero)
        {
            Fsm.TransitionTo(inputToIdle);
        }

        if (PlayerInputs.Instance.Run())
        {
            Fsm.TransitionTo(inputToRun);
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
