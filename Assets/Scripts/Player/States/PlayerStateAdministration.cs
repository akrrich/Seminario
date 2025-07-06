using UnityEngine;

public class PlayerStateAdministration<T> : State<T>
{
    private PlayerModel playerModel;
    private PlayerView playerView;
    private Transform administratingPosition;

    private T inputToIdle;


    public PlayerStateAdministration(T inputToIdle, PlayerModel playerModel, PlayerView playerView)
    {
        this.inputToIdle = inputToIdle;
        this.playerModel = playerModel;
        this.playerView = playerView;

        administratingPosition = GameObject.Find("AdministratingPosition").transform;
    }


    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Cook");

        PlayerView.OnEnterInAdministrationMode?.Invoke();
        PlayerView.OnDeactivateInventoryFoodUI?.Invoke();

        playerModel.Rb.velocity = Vector3.zero;
        playerModel.CapsuleCollider.material = null;

        playerView.ShowOrHideDish(false);
        playerModel.IsAdministrating = true;
        playerModel.transform.position = administratingPosition.transform.position;
        playerModel.LookAt(playerModel.Administration.transform.position);
        playerModel.PlayerCamera.transform.localEulerAngles = new Vector3(-1, 0, 0);
    }

    public override void Execute()
    {
        base.Execute();

        if (PlayerInputs.Instance.Administration())
        {
            Fsm.TransitionTo(inputToIdle);
        }
    }

    public override void Exit()
    {
        base.Exit();

        PlayerView.OnExitInAdministrationMode?.Invoke();
        playerView.ShowOrHideDish(true);
        playerModel.IsAdministrating = false;

        playerModel.CapsuleCollider.material = playerModel.PhysicsMaterial;
    }
}
