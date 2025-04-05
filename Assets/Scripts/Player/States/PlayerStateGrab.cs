using UnityEngine;

public class PlayerStateGrab<T> : State<T>
{
    private PlayerModel playerModel;

    private T inputToIdle;
    private T inputToWalk;


    public PlayerStateGrab(T inputToIdle, T inputToWalk, PlayerModel playerModel)
    {
        this.inputToIdle = inputToIdle;
        this.inputToWalk = inputToWalk;
        this.playerModel = playerModel;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Grab");

        playerModel.InventoryManager.SaveElementInInventory(playerModel.CurrentItem, playerModel.Inventory);
        playerModel.CurrentItem = null;
    }

    public override void Execute()
    {
        base.Execute();

        if (playerModel.GetMoveAxis() == Vector2.zero)
        {
            Fsm.TransitionTo(inputToIdle);
        }

        if (playerModel.GetMoveAxis() != Vector2.zero)
        {
            Fsm.TransitionTo(inputToWalk);
        }
    }

    public override void Exit()
    {
        playerModel.IsCollidingItem = false;
    }
}
