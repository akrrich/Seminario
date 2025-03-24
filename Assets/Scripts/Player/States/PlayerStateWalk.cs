using UnityEngine;

public class PlayerStateWalk<T> : State<T>
{
    private PlayerModel playerModel;

    private T inputToIdle;
    private T inputToJump;
    private T inputToCook;


    public PlayerStateWalk(T inputToIdle, T inputToJump, T inputToCook, PlayerModel playerModel)
    {
        this.inputToIdle = inputToIdle;
        this.inputToJump = inputToJump;
        this.inputToCook = inputToCook;
        this.playerModel = playerModel;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Walk");
    }

    public override void Execute()
    {
        base.Execute();

        Vector3 direction = new Vector3(PlayerModel.GetMoveAxis().x, 0, PlayerModel.GetMoveAxis().y);

        if (direction == Vector3.zero)
        {
            Fsm.TransitionTo(inputToIdle);
        }

        if (Input.GetKeyDown(KeyCode.Space) && playerModel.IsGrounded)
        {
            Fsm.TransitionTo(inputToJump);
        }

        if (Input.GetKeyDown(KeyCode.E) && playerModel.IsCollidingOven)
        {
            Fsm.TransitionTo(inputToCook);
        }
    }
}
