using UnityEngine;

public class PlayerStateRun<T> : State<T>
{
    private PlayerModel playerModel;

    private T inputToIdle;
    private T inputToWalk;
    private T inputToJump;
    private T inputToCook;


    public PlayerStateRun(T inputToIdle, T inputToWalk, T inputToJump, T inputToCook, PlayerModel playerModel)
    {
        this.inputToIdle = inputToIdle;
        this.inputToWalk = inputToWalk;
        this.inputToJump = inputToJump;
        this.inputToCook = inputToCook;
        this.playerModel = playerModel;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Run");

        playerModel.Speed = playerModel.RunSpeed;
    }

    public override void Execute()
    {
        base.Execute();

        if (playerModel.GetMoveAxis() == Vector2.zero)
        {
            Fsm.TransitionTo(inputToIdle);
        }

        if (PlayerInputs.Instance.StopRun())
        {
            Fsm.TransitionTo(inputToWalk);
        }

        if (PlayerInputs.Instance.Jump())
        {
            Fsm.TransitionTo(inputToJump);
        }

        if (PlayerInputs.Instance.Cook() && playerModel.IsCollidingOven && playerModel.IsLookingAtOven())
        {
            Fsm.TransitionTo(inputToCook);
        }
    }
}
