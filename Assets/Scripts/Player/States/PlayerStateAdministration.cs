using UnityEngine;

public class PlayerStateAdministration<T> : State<T>
{
    private PlayerModel playerModel;
    private PlayerView playerView;

    private GameObject administration;
    private Transform administratingPosition;

    private bool lastDishState;

    private T inputToIdle;


    public PlayerStateAdministration(T inputToIdle, PlayerModel playerModel, PlayerView playerView)
    {
        this.inputToIdle = inputToIdle;
        this.playerModel = playerModel;
        this.playerView = playerView;

        administration = GameObject.FindGameObjectWithTag("Administration");
        administratingPosition = GameObject.Find("AdministratingPosition").transform;
    }


    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Administration");

        PlayerView.OnEnterInAdministrationMode?.Invoke();

        playerModel.Rb.velocity = Vector3.zero;
        playerModel.CapsuleCollider.material = null;

        lastDishState = playerView.Dish.gameObject.activeSelf;

        playerView.ShowOrHideDish(false);
        playerModel.transform.position = administratingPosition.transform.position;
        playerModel.LookAt(administration.transform.position);
        playerModel.PlayerCamera.transform.localEulerAngles = new Vector3(-1, 0, 0);
    }

    public override void Execute()
    {
        base.Execute();

        if (PlayerInputs.Instance.InteractPress())
        {
            Fsm.TransitionTo(inputToIdle);
        }
    }

    public override void Exit()
    {
        base.Exit();

        PlayerView.OnExitInAdministrationMode?.Invoke();
        playerView.ShowOrHideDish(lastDishState);
        playerModel.IsAdministrating = false;

        playerModel.CapsuleCollider.material = playerModel.PhysicsMaterial;
    }
}
