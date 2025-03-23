using UnityEngine;

public class PlayerStateJump<T> : State<T>
{
    private PlayerModel playerModel;

    private T inputToIdle;

    private float jumpForce = 5;


    public PlayerStateJump(T inputToIdle, PlayerModel playerModel)
    {
        this.inputToIdle = inputToIdle;
        this.playerModel = playerModel;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Jump");

        playerModel.Rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public override void Execute()
    {
        base.Execute();

        if (playerModel.IsGrounded)
        {
            Fsm.TransitionTo(inputToIdle);
        }
    }
}
