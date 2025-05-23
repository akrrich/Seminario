using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerModel playerModel;

    private FSM<PlayerStates> fsm = new FSM<PlayerStates>();
    private PlayerCollisions playerCollisions;

    private static event Action onGrabFood;
    private static event Action onHandOverFood;

    private static event Action onTakeOrder;

    // Estos 2 eventos corresponden a Entregar el plato una vez tomado el pedido
    private static event Action<Table> onTableCollisionEnterForHandOverFood;
    private static event Action onTableCollisionExitForHandOverFood;

    // Estos 2 eventos corresponden a Tomar el pedido de un cliente
    private static event Action<Table> onTableCollisionEnterForTakeOrder;
    private static event Action onTableCollisionExitForTakeOrder;

    public PlayerModel PlayerModel { get => playerModel; }

    public static Action OnGrabFood { get => onGrabFood; set => onGrabFood = value; }
    public static Action OnHandOverFood { get => onHandOverFood; set => onHandOverFood = value; }

    public static Action OnTakeOrder { get => onTakeOrder; set => onTakeOrder = value; }

    public static Action<Table> OnTableCollisionEnterToHandOverFood { get => onTableCollisionEnterForHandOverFood; set => onTableCollisionEnterForHandOverFood = value; }
    public static Action OnTableCollisionExitForHandOver { get => onTableCollisionExitForHandOverFood; set => onTableCollisionExitForHandOverFood = value; }

    public static Action<Table> OnTableCollisionEnterForTakeOrder { get => onTableCollisionEnterForTakeOrder; set => onTableCollisionEnterForTakeOrder = value; }
    public static Action OnTableCollisionExitForTakeOrder { get => onTableCollisionExitForTakeOrder; set => onTableCollisionExitForTakeOrder = value; }


    void Awake()
    {
        GetComponentsAndInitializeReferences();
        InitializeFSM();
    }

    void Update()
    {
        fsm.OnExecute();
        GrabOrHandOverFood();
        TakeClientOrder();
    }

    void OnCollisionEnter(Collision collision)
    {
        playerCollisions.OnCollisionEnterWithFloor(collision);
        playerCollisions.OnCollisionEnterWithOvenAndLOS(collision);
        playerCollisions.OnCollisionEnterWithTable(collision);
        playerCollisions.OnCollisionEnterWithAdministration(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        playerCollisions.OnCollisionStayWithOvenAndLOS(collision);
        playerCollisions.OnCollisionStayWithAdministrationAndLOS(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        playerCollisions.OnCollisionExitWithFloor(collision);
        playerCollisions.OnCollisionExitWithOven(collision);
        playerCollisions.OnCollisionExitWithTable(collision);
        playerCollisions.OnCollisionExitWithAdministration(collision);
    }


    private void GetComponentsAndInitializeReferences()
    {
        playerModel = GetComponent<PlayerModel>();
        playerCollisions = new PlayerCollisions(this);
    }

    private void InitializeFSM() 
    {
        PlayerStateIdle<PlayerStates> psIdle = new PlayerStateIdle<PlayerStates>(PlayerStates.Walk, PlayerStates.Jump, PlayerStates.Cook, PlayerStates.Admin, playerModel);
        PlayerStateWalk<PlayerStates> psWalk = new PlayerStateWalk<PlayerStates> (PlayerStates.Idle, PlayerStates.Run, PlayerStates.Jump, PlayerStates.Cook, PlayerStates.Admin, playerModel);
        PlayerStateJump<PlayerStates> psJump = new PlayerStateJump<PlayerStates>(PlayerStates.Idle, playerModel);
        PlayerStateCook<PlayerStates> psCook = new PlayerStateCook<PlayerStates>(PlayerStates.Idle, playerModel);
        PlayerStateRun<PlayerStates> psRun = new PlayerStateRun<PlayerStates>(PlayerStates.Idle, PlayerStates.Walk, PlayerStates.Jump, PlayerStates.Cook, PlayerStates.Admin, playerModel);
        PlayerStateAdministration<PlayerStates> psAdmin = new PlayerStateAdministration<PlayerStates>(PlayerStates.Idle, playerModel);

        psIdle.AddTransition(PlayerStates.Walk, psWalk);
        psIdle.AddTransition(PlayerStates.Jump, psJump);
        psIdle.AddTransition(PlayerStates.Cook, psCook);
        psIdle.AddTransition(PlayerStates.Admin, psAdmin);

        psWalk.AddTransition(PlayerStates.Idle, psIdle);
        psWalk.AddTransition(PlayerStates.Jump, psJump);
        psWalk.AddTransition(PlayerStates.Cook, psCook);
        psWalk.AddTransition(PlayerStates.Run, psRun);
        psWalk.AddTransition(PlayerStates.Admin, psAdmin);

        psJump.AddTransition(PlayerStates.Idle, psIdle);
        psJump.AddTransition(PlayerStates.Walk, psWalk);

        psCook.AddTransition(PlayerStates.Idle, psIdle);

        psRun.AddTransition(PlayerStates.Idle, psIdle);
        psRun.AddTransition(PlayerStates.Walk, psWalk);
        psRun.AddTransition(PlayerStates.Jump, psJump);
        psRun.AddTransition(PlayerStates.Cook, psCook);
        psRun.AddTransition(PlayerStates.Admin, psAdmin);

        psAdmin.AddTransition(PlayerStates.Idle, psIdle);

        fsm.SetInit(psIdle);
    }

    private void GrabOrHandOverFood()
    {
        if (PlayerInputs.Instance != null)
        {
            if (PlayerInputs.Instance.GrabFood())
            {
                onGrabFood?.Invoke();
            }

            if (PlayerInputs.Instance.HandOverFood())
            {
                onHandOverFood?.Invoke();
            }
        }
    }

    private void TakeClientOrder()
    {
        if (PlayerInputs.Instance != null)
        {
            if (PlayerInputs.Instance.TakeClientOrder())
            {
                onTakeOrder?.Invoke();
            }
        }
    }
}
