using UnityEngine;

public class PlayerStateIdle<T> : State<T>
{
    private PlayerModel playerModel;

    private T inputToWalk;
    private T inputToJump;
    private T inputToCook;
    private T inputToGrab;


    public PlayerStateIdle(T inputToWalk, T inputToJump, T inputToCook, T inputToGrab, PlayerModel playerModel)
    {
        this.inputToWalk = inputToWalk;
        this.inputToJump = inputToJump;
        this.inputToCook = inputToCook;
        this.inputToGrab = inputToGrab;
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

        if (Input.GetKeyDown(KeyCode.Space) && playerModel.IsGrounded)
        {
            Fsm.TransitionTo(inputToJump);
        }

        if (Input.GetKeyDown(KeyCode.E) && playerModel.IsCollidingOven && playerModel.IsLookingAtOven())
        {
            Fsm.TransitionTo(inputToCook);
        }

        if (Input.GetKeyDown(KeyCode.R) && playerModel.IsCollidingItem)
        {
            Fsm.TransitionTo(inputToGrab);
        }
    }
}
