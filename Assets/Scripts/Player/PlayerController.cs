using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Teclas ocupadas: WASD, Q, E, R, X, Z, indicacion en la clase PlayerInputs

    private PlayerModel playerModel;

    private FSM<PlayerStates> fsm = new FSM<PlayerStates>();
    private PlayerCollisions playerCollisions;

    private static event Action onGrabFood;
    private static event Action onHandOverFood;

    private static event Action<Table> onTableCollisionEnter;
    private static event Action onTableCollisionExit;

    public PlayerModel PlayerModel { get => playerModel; }

    public static Action OnGrabFood { get => onGrabFood; set => onGrabFood = value; }
    public static Action OnHandOverFood { get => onHandOverFood; set => onHandOverFood = value; }

    public static Action<Table> OnTableCollisionEnter { get => onTableCollisionEnter; set => onTableCollisionEnter = value; }
    public static Action OnTableCollisionExit { get => onTableCollisionExit; set => onTableCollisionExit = value; }


    void Awake()
    {
        GetComponentsAndInitializeReferences();
        InitializeFSM();
    }

    void Update()
    {
        fsm.OnExecute();

        GrabOrDropItems();
        GrabOrHandOverFood();
    }

    void OnCollisionEnter(Collision collision)
    {
        playerCollisions.OnCollisionEnterWithFloor(collision);
        playerCollisions.OnCollisionEnterWithOvenAndLOS(collision);
        playerCollisions.OnCollisionEnterWithItem(collision);
        playerCollisions.OnCollisionEnterWithTable(collision);
        playerCollisions.OnCollisionEnterWithAdministration(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        playerCollisions.OnCollisionStayWithOvenAndLOS(collision);
        playerCollisions.OnCollisionStayWithItem(collision);
        playerCollisions.OnCollisionStayWithAdministrationAndLOS(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        playerCollisions.OnCollisionExitWithFloor(collision);
        playerCollisions.OnCollisionExitWithOven(collision);
        playerCollisions.OnCollisionExitWithItem(collision);
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

    private void GrabOrDropItems()
    {
        if (PlayerInputs.Instance.GrabItem() && playerModel.IsCollidingItem)
        {
            playerModel.InventoryManager.SaveElementInInventory(playerModel.CurrentItem, playerModel.Inventory);
            playerModel.CurrentItem = null;
            playerModel.IsCollidingItem = false;
        }

        if (PlayerInputs.Instance.DropItem() && playerModel.Inventory.childCount > 0) 
        {
            playerModel.InventoryManager.RemoveElmentFromInventory(playerModel.Inventory);
        }
    }

    private void GrabOrHandOverFood()
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
