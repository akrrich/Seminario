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

        Vector3 direction = new Vector3(PlayerModel.GetMoveAxis().x, 0, PlayerModel.GetMoveAxis().y);

        if (direction == Vector3.zero)
        {
            Fsm.TransitionTo(inputToIdle);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Fsm.TransitionTo(inputToWalk);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fsm.TransitionTo(inputToJump);
        }

        if (Input.GetKeyDown(KeyCode.E) && playerModel.IsCollidingOven && playerModel.IsLookingAtOven())
        {
            Fsm.TransitionTo(inputToCook);
        }
    }
}
