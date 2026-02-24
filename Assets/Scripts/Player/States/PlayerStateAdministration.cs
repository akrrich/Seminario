using UnityEngine;

public class PlayerStateAdministration<T> : State<T>
{
    private PlayerModel playerModel;
    private PlayerView playerView;

    private bool lastDishState;

    private T inputToIdle;
    private bool ignoreInputThisFrame = false;

    public PlayerStateAdministration(T inputToIdle, PlayerModel playerModel, PlayerView playerView)
    {
        this.inputToIdle = inputToIdle;
        this.playerModel = playerModel;
        this.playerView = playerView;
    }


    public override void Enter()
    {
        base.Enter();
        AdministratingManagerUI.OnExitAdmin += OnExitStateWhenClickOnButtonCloseUI;
        PauseManager.OnGameUnPaused += OnGameUnPaused;
        PlayerView.OnEnterInAdministrationMode?.Invoke();
        playerModel.Rb.velocity = Vector3.zero;
        playerModel.CapsuleCollider.material = null;
        lastDishState = playerView.Dish.gameObject.activeSelf;
        playerView.ShowOrHideDish(false);
    }

    public override void Execute()
    {
        base.Execute();

        if (PauseManager.Instance != null && PauseManager.Instance.IsGamePaused)
            return;

        if (PlayerInputs.Instance.InteractPress() || PlayerInputs.Instance.BackPanelsUI())
        {
            Fsm.TransitionTo(inputToIdle);
        }
    }

    public override void Exit()
    {
        base.Exit();
        AdministratingManagerUI.OnExitAdmin -= OnExitStateWhenClickOnButtonCloseUI;
        PauseManager.OnGameUnPaused -= OnGameUnPaused;
        PlayerView.OnExitInAdministrationMode?.Invoke();
        playerView.ShowOrHideDish(lastDishState);
        playerModel.IsAdministrating = false;
        playerModel.CapsuleCollider.material = playerModel.PhysicsMaterial;
    }

    // Obligatorio para llamar cuando el player se destruye
    public void UnsuscribeToEventWhenPlayerDestroy()
    {
        AdministratingManagerUI.OnExitAdmin -= OnExitStateWhenClickOnButtonCloseUI;
        PauseManager.OnGameUnPaused -= OnGameUnPaused;
    }
    private void OnGameUnPaused()
    {
        ignoreInputThisFrame = true;
    }

    private void OnExitStateWhenClickOnButtonCloseUI()
    {
        Fsm.TransitionTo(inputToIdle);
    }
}
