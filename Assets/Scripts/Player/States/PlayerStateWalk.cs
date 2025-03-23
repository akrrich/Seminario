using UnityEngine;

public class PlayerStateWalk<T> : State<T>
{
    private PlayerModel playerModel;

    private T inputToIdle;
    private T inputToJump;


    public PlayerStateWalk(T inputToIdle, T inputToJump, PlayerModel playerModel)
    {
        this.inputToIdle = inputToIdle;
        this.inputToJump = inputToJump;
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

        Vector3 direction = new Vector3(PlayerModel.GetMove().x, 0, PlayerModel.GetMove().y);

        if (direction == Vector3.zero)
        {
            Fsm.TransitionTo(inputToIdle);
        }

        if (Input.GetKeyDown(KeyCode.Space) && playerModel.IsGrounded)
        {
            Fsm.TransitionTo(inputToJump);
        }
    }
}
