using UnityEngine;

public class PlayerStateCook<T> : State<T>
{
    private PlayerModel playerModel;

    private T inputToIdle;


    public PlayerStateCook(T inputToIdle, PlayerModel playerModel)
    {
        this.inputToIdle = inputToIdle;
        this.playerModel = playerModel;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Cook");

        PlayerView.OnEnterInCookMode?.Invoke();
        PlayerView.OnDeactivateInventoryFoodUI?.Invoke();
        playerModel.ShowOrHideDish(false);
        playerModel.IsCooking = true;
        playerModel.transform.position = playerModel.CookingPosition.transform.position;
        playerModel.transform.rotation = Quaternion.Euler(0, -90, 0);
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
        playerModel.ShowOrHideDish(true);
        playerModel.IsCooking = false;
    }
}
