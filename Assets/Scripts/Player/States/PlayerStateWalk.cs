using UnityEngine;

public class PlayerStateWalk<T> : State<T>
{
    private PlayerModel playerModel;

    private T inputToIdle;
    private T inputToRun;
    private T inputToJump;
    private T inputToCook;
    private T inputToGrab;


    public PlayerStateWalk(T inputToIdle, T inputToRun, T inputToJump, T inputToCook, T inputToGrab, PlayerModel playerModel)
    {
        this.inputToIdle = inputToIdle;
        this.inputToRun = inputToRun;
        this.inputToJump = inputToJump;
        this.inputToCook = inputToCook;
        this.inputToGrab = inputToGrab;
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

        Vector3 direction = new Vector3(playerModel.GetMoveAxis().x, 0, playerModel.GetMoveAxis().y);

        if (direction == Vector3.zero)
        {
            Fsm.TransitionTo(inputToIdle);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Fsm.TransitionTo(inputToRun);
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
