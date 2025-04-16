using UnityEngine;

public class PlayerStateAdministration<T> : State<T>
{
    private PlayerModel playerModel;

    private T inputToIdle;


    public PlayerStateAdministration(T inputToIdle, PlayerModel playerModel)
    {
        this.inputToIdle = inputToIdle;
        this.playerModel = playerModel;
    }


    public override void Enter()
    {
        base.Enter();
        Debug.Log("Cook");

        PlayerView.OnEnterInAdministrationMode?.Invoke();
        playerModel.ShowOrHideDish(false);
        playerModel.IsAdministrating = true;
        playerModel.transform.position = playerModel.AdministratingPosition.transform.position;
        playerModel.transform.rotation = Quaternion.Euler(0, 90, 0);
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
        playerModel.ShowOrHideDish(true);
        playerModel.IsAdministrating = false;
    }
}
