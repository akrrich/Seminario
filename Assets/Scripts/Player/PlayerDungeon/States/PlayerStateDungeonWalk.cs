using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDungeonWalk<T> : State<T>
{
    private PlayerDungeonModel dungeonModel;
    private PlayerDungeonView dungeonView;

    private T idleInput;
    private T runInput;
    private T jumpInput;
    private T combatInput;
    private T dashInput;

    public PlayerStateDungeonWalk(
        PlayerDungeonModel dungeonModel,
        PlayerDungeonView dungeonView,
        T idleInput, T runInput, T jumpInput, T combatInput, T dashInput)
    {
        this.dungeonModel = dungeonModel;
        this.dungeonView = dungeonView;

        this.idleInput = idleInput;
        this.runInput = runInput;
        this.jumpInput = jumpInput;
        this.combatInput = combatInput;
        this.dashInput = dashInput;
    }
    public override void Enter()
    {
        base.Enter();
        dungeonView.PlayWalkAnimation(true);
        dungeonModel.Speed = dungeonModel.WalkSpeed;
    }

    public override void Execute()
    {
        base.Execute();

        if (PlayerInputs.Instance == null) return;

        if (dungeonModel.IsDead)
        {
            Fsm.TransitionTo((T)(object)PlayerPhase.Dead);
            return;
        }

        if (PlayerInputs.Instance.Dash() && dungeonModel.CanDash)
        {
            Fsm.TransitionTo(dashInput);
            return;
        }

        if (PlayerInputs.Instance.Attack())
        {
            Fsm.TransitionTo(combatInput);
            return;
        }

        if (PlayerInputs.Instance.Jump() && dungeonModel.IsGrounded)
        {
            Fsm.TransitionTo(jumpInput);
            return;
        }

        Vector2 moveInput = PlayerInputs.Instance.GetMoveAxis();

        if (moveInput.magnitude < 0.1f)
        {
            Fsm.TransitionTo(idleInput);
            return;
        }

        if (PlayerInputs.Instance.Run())
        {
            Fsm.TransitionTo(runInput);
            return;
        }

        // Update direction — model will handle actual movement
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = Camera.main.transform.right;
        Vector3 inputDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        dungeonModel.MoveDirection = inputDirection;
        dungeonModel.LookAt(inputDirection);
    }

    public override void Exit()
    {
        base.Exit();
        dungeonView.PlayWalkAnimation(false);
    }
}
