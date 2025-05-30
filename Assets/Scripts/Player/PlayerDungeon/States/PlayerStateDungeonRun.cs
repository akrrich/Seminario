using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDungeonRun<T> : State<T>
{
    private PlayerDungeonModel dungeonModel;
    private PlayerDungeonView dungeonView;
    private T idleInput;
    private T walkInput;
    private T jumpInput;

    public PlayerStateDungeonRun(
        PlayerDungeonModel dungeonModel,
        PlayerDungeonView dungeonView,
        T idleInput, T walkInput, T jumpInput)
    {
        this.dungeonModel = dungeonModel;
        this.dungeonView = dungeonView;
        this.idleInput = idleInput;
        this.walkInput = walkInput;
        this.jumpInput = jumpInput;
    }
    public override void Enter()
    {
        base.Enter();
        dungeonModel.Speed = dungeonModel.RunSpeed;
        //dungeonView.PlayRunAnimation(true);
    }

    public override void Execute()
    {
        base.Execute();
        if (PlayerInputs.Instance == null) return;

        Vector2 moveInput = PlayerInputs.Instance.GetMoveAxis();

        if (dungeonModel.IsDead)
        {
            Fsm.TransitionTo((T)(object)PlayerPhase.Dead);
            return;
        }

        if (moveInput.magnitude < 0.1f)
        {
            Fsm.TransitionTo(idleInput);
            return;
        }

        // Since PlayerInputs.Run() uses GetKeyDown, we add a persistent check here.
        if (!Input.GetKey(PlayerInputs.Instance.KeyboardInputs.Run) &&
            !Input.GetKey(PlayerInputs.Instance.JoystickInputs.Run))
        {
            Fsm.TransitionTo(walkInput);
            return;
        }

        if (PlayerInputs.Instance.Jump() && dungeonModel.IsGrounded)
        {
            Fsm.TransitionTo(jumpInput);
            return;
        }

        // Directional movement + look
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 right = Camera.main.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 movement = (cameraForward * moveInput.y + right * moveInput.x).normalized;
        dungeonModel.MoveDirection = movement;

        if (movement.sqrMagnitude > 0.01f)
        {
            dungeonModel.LookAt(movement);
        }
    }

    public override void Exit()
    {
        base.Exit();
        dungeonModel.Speed = 0f;
        //dungeonView.PlayRunAnimation(false);
    }
}
