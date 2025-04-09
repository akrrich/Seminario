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

    public PlayerModel PlayerModel { get => playerModel; }

    public static Action OnGrabFood { get => onGrabFood; set => onGrabFood = value; }
    public static Action OnHandOverFood { get => onHandOverFood; set => onHandOverFood = value; }


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

        // Provisorio hay que solucionar el lineofsight
        if (!playerModel.IsLookingAtOven())
        {
            PlayerView.OnExitInCookModeMessage?.Invoke();
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


    private void GetComponentsAndInitializeReferences()
    {
        playerModel = GetComponent<PlayerModel>();
        playerCollisions = new PlayerCollisions(this);
    }

    private void InitializeFSM() 
    {
        PlayerStateIdle<PlayerStates> psIdle = new PlayerStateIdle<PlayerStates>(PlayerStates.Walk, PlayerStates.Jump, PlayerStates.Cook, playerModel);
        PlayerStateWalk<PlayerStates> psWalk = new PlayerStateWalk<PlayerStates> (PlayerStates.Idle, PlayerStates.Run, PlayerStates.Jump, PlayerStates.Cook, playerModel);
        PlayerStateJump<PlayerStates> psJump = new PlayerStateJump<PlayerStates>(PlayerStates.Idle, playerModel);
        PlayerStateCook<PlayerStates> psCook = new PlayerStateCook<PlayerStates>(PlayerStates.Idle, playerModel);
        PlayerStateRun<PlayerStates> psRun = new PlayerStateRun<PlayerStates>(PlayerStates.Idle, PlayerStates.Walk, PlayerStates.Jump, PlayerStates.Cook, playerModel);
         
        psIdle.AddTransition(PlayerStates.Walk, psWalk);
        psIdle.AddTransition(PlayerStates.Jump, psJump);
        psIdle.AddTransition(PlayerStates.Cook, psCook);

        psWalk.AddTransition(PlayerStates.Idle, psIdle);
        psWalk.AddTransition(PlayerStates.Jump, psJump);
        psWalk.AddTransition(PlayerStates.Cook, psCook);
        psWalk.AddTransition(PlayerStates.Run, psRun);

        psJump.AddTransition(PlayerStates.Idle, psIdle);
        psJump.AddTransition(PlayerStates.Walk, psWalk);

        psCook.AddTransition(PlayerStates.Idle, psIdle);

        psRun.AddTransition(PlayerStates.Idle, psIdle);
        psRun.AddTransition(PlayerStates.Walk, psWalk);
        psRun.AddTransition(PlayerStates.Jump, psJump);
        psRun.AddTransition(PlayerStates.Cook, psCook);

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
