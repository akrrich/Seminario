using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerModel playerModel;

    private FSM<PlayerStates> fsm = new FSM<PlayerStates>();


    void Awake()
    {
        InitializeFSM();
    }

    void Update()
    {
        fsm.OnExecute();

        // Provisorio
        if (!IsLookingAtOven())
        {
            PlayerView.OnExitInCookModeMessage?.Invoke();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        ColisionEnterWithFloor(collision);
        OnCollisionEnterWithOven(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        OnCollisionStayWithOvenAndLOS(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        ColisionExitWithFloor(collision);
        OnColisionExitWithOven(collision);
    }


    private void InitializeFSM()
    {
        PlayerStateIdle<PlayerStates> psIdle = new PlayerStateIdle<PlayerStates>(PlayerStates.Walk, PlayerStates.Jump, PlayerStates.Cook, playerModel);
        PlayerStateWalk<PlayerStates> psWalk = new PlayerStateWalk<PlayerStates> (PlayerStates.Idle, PlayerStates.Jump, PlayerStates.Cook, playerModel);
        PlayerStateJump<PlayerStates> psJump = new PlayerStateJump<PlayerStates>(PlayerStates.Idle, playerModel);
        PlayerStateCook<PlayerStates> psCook = new PlayerStateCook<PlayerStates>(PlayerStates.Idle, playerModel);

        psIdle.AddTransition(PlayerStates.Walk, psWalk);
        psIdle.AddTransition(PlayerStates.Jump, psJump);
        psIdle.AddTransition(PlayerStates.Cook, psCook);

        psWalk.AddTransition(PlayerStates.Idle, psIdle);
        psWalk.AddTransition(PlayerStates.Jump, psJump);
        psWalk.AddTransition(PlayerStates.Cook, psCook);

        psJump.AddTransition(PlayerStates.Idle, psIdle);
        psJump.AddTransition(PlayerStates.Walk, psWalk);

        psCook.AddTransition(PlayerStates.Idle, psIdle);

        fsm.SetInit(psIdle);
    }

    private void ColisionEnterWithFloor(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            playerModel.IsGrounded = true;
        }
    }

    private void OnCollisionEnterWithOven(Collision collision)
    {
        if (collision.gameObject.CompareTag("Oven") && IsLookingAtOven())
        {
            playerModel.IsCollidingOven = true;
            PlayerView.OnEnterInCookModeMessage?.Invoke();
        }
    }

    private void OnCollisionStayWithOvenAndLOS(Collision collision)
    {
        if (collision.gameObject.CompareTag("Oven") && IsLookingAtOven())
        {
            playerModel.IsCollidingOven = true;
            PlayerView.OnEnterInCookModeMessage?.Invoke();
        }
    }

    private void ColisionExitWithFloor(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            playerModel.IsGrounded = false;
        }
    }

    private void OnColisionExitWithOven(Collision colision)
    {
        if (colision.gameObject.CompareTag("Oven"))
        {
            playerModel.IsCollidingOven = false;
            PlayerView.OnExitInCookModeMessage?.Invoke();
        }
    }

    // Provisorio
    private bool IsLookingAtOven()
    {
        GameObject oven = GameObject.FindGameObjectWithTag("Oven");

        if (oven == null) return false;

        Vector3 directionToOven = oven.transform.position - playerModel.transform.position;
        float angle = Vector3.Angle(playerModel.PlayerCamera.transform.forward, directionToOven);

        if (angle <= 90f) 
        {
            return true;
        }

        return false;
    }
}
