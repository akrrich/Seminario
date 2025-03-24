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

        if (PlayerModel.GetMoveAxis() != Vector2.zero)
        {
            Fsm.TransitionTo(inputToWalk);
        }

        if (Input.GetKeyDown(KeyCode.Space) && playerModel.IsGrounded)
        {
            Fsm.TransitionTo(inputToJump);
        }

        if (Input.GetKeyDown(KeyCode.E) && playerModel.IsCollidingOven && IsLookingAtOven())
        {
            Fsm.TransitionTo(inputToCook);
        }
    }

    public bool IsLookingAtOven()
    {
        GameObject oven = GameObject.FindGameObjectWithTag("Oven");

        Vector3 directionToOven = oven.transform.position - playerModel.transform.position;
        float angle = Vector3.Angle(playerModel.PlayerCamera.transform.forward, directionToOven);

        return LineOfSight.CheckRange(playerModel.PlayerCamera.transform, oven.transform, angle);
    }
}
