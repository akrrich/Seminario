using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Teclas ocupadas provisoriamente: WASD, Q, E, R, X, Z

    [SerializeField] private PlayerModel playerModel;

    private FSM<PlayerStates> fsm = new FSM<PlayerStates>();
    private PlayerCollisions playerCollisions;


    public PlayerModel PlayerModel { get => playerModel; }


    void Awake()
    {
        InitializeFSM();
        playerCollisions = new PlayerCollisions(this);
    }

    void Update()
    {
        fsm.OnExecute();

        // Provisorio
        if (!playerModel.IsLookingAtOven())
        {
            PlayerView.OnExitInCookModeMessage?.Invoke();
        }

        // Provisorio
        if (Input.GetKeyDown(KeyCode.X))
        {
            playerModel.InventoryManager.RemoveElmentFromInventory(playerModel.Inventory);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        playerCollisions.OnCollisionEnterWithFloor(collision);
        playerCollisions.OnCollisionEnterWithOvenAndLOS(collision);
        playerCollisions.OnCollisionEnterWithItem(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        playerCollisions.OnCollisionStayWithOvenAndLOS(collision);
        playerCollisions.OnCollisionStayWithItem(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        playerCollisions.OnCollisionExitWithFloor(collision);
        playerCollisions.OnCollisionExitWithOven(collision);
        playerCollisions.OnCollisionExitWithItem(collision);
    }


    private void InitializeFSM()
    {
        PlayerStateIdle<PlayerStates> psIdle = new PlayerStateIdle<PlayerStates>(PlayerStates.Walk, PlayerStates.Jump, PlayerStates.Cook, PlayerStates.Grab, playerModel);
        PlayerStateWalk<PlayerStates> psWalk = new PlayerStateWalk<PlayerStates> (PlayerStates.Idle, PlayerStates.Run, PlayerStates.Jump, PlayerStates.Cook, PlayerStates.Grab, playerModel);
        PlayerStateJump<PlayerStates> psJump = new PlayerStateJump<PlayerStates>(PlayerStates.Idle, playerModel);
        PlayerStateCook<PlayerStates> psCook = new PlayerStateCook<PlayerStates>(PlayerStates.Idle, playerModel);
        PlayerStateGrab<PlayerStates> psGrab = new PlayerStateGrab<PlayerStates>(PlayerStates.Idle, PlayerStates.Walk, playerModel);
        PlayerStateRun<PlayerStates> psRun = new PlayerStateRun<PlayerStates>(PlayerStates.Idle, PlayerStates.Walk, PlayerStates.Jump, PlayerStates.Cook, playerModel);

        psIdle.AddTransition(PlayerStates.Walk, psWalk);
        psIdle.AddTransition(PlayerStates.Jump, psJump);
        psIdle.AddTransition(PlayerStates.Cook, psCook);
        psIdle.AddTransition(PlayerStates.Grab, psGrab);

        psWalk.AddTransition(PlayerStates.Idle, psIdle);
        psWalk.AddTransition(PlayerStates.Jump, psJump);
        psWalk.AddTransition(PlayerStates.Cook, psCook);
        psWalk.AddTransition(PlayerStates.Grab, psGrab);
        psWalk.AddTransition(PlayerStates.Run, psRun);

        psJump.AddTransition(PlayerStates.Idle, psIdle);
        psJump.AddTransition(PlayerStates.Walk, psWalk);

        psCook.AddTransition(PlayerStates.Idle, psIdle);

        psGrab.AddTransition(PlayerStates.Idle, psIdle);
        psGrab.AddTransition(PlayerStates.Walk, psWalk);

        psRun.AddTransition(PlayerStates.Idle, psIdle);
        psRun.AddTransition(PlayerStates.Walk, psWalk);
        psRun.AddTransition(PlayerStates.Jump, psJump);
        psRun.AddTransition(PlayerStates.Cook, psCook);

        fsm.SetInit(psIdle);
    }
}
