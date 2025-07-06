using UnityEngine;

public class PlayerStateCook<T> : State<T>
{
    private PlayerModel playerModel;
    private PlayerView playerView;
    private Transform cookingPosition;

    private T inputToIdle;


    public PlayerStateCook(T inputToIdle, PlayerModel playerModel, PlayerView playerView)
    {
        this.inputToIdle = inputToIdle;
        this.playerModel = playerModel;
        this.playerView = playerView;

        cookingPosition = GameObject.Find("CookingPosition").transform;
    }

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Cook");

        PlayerView.OnEnterInCookMode?.Invoke();
        PlayerView.OnDeactivateInventoryFoodUI?.Invoke();

        playerModel.Rb.velocity = Vector3.zero;
        playerModel.CapsuleCollider.material = null;

        playerView.ShowOrHideDish(false);
        playerModel.IsCooking = true;
        playerModel.transform.position = cookingPosition.transform.position;
        playerModel.LookAt(playerModel.CookingDeskUI.transform.position);
        playerModel.PlayerCamera.transform.localEulerAngles = new Vector3(-1, 0, 0);
    }

    public override void Execute()
    {
        base.Execute();

        if (PlayerInputs.Instance.Cook())
        {
            Fsm.TransitionTo(inputToIdle);
        }
    }

    public override void Exit()
    {
        PlayerView.OnExitInCookMode?.Invoke();
        playerView.ShowOrHideDish(true);
        playerModel.IsCooking = false;

        playerModel.CapsuleCollider.material = playerModel.PhysicsMaterial;
    }
}
