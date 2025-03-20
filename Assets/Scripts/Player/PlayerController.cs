using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private FSM<PlayerStates> fsm = new FSM<PlayerStates>();

    private Rigidbody rb;


    public Rigidbody Rb { get => rb; }


    void Awake()
    {
        InitializeFSM();

        rb = GetComponent<Rigidbody>();
    }


    private void InitializeFSM()
    {
        PlayerStateIdle<PlayerStates> psIdle = new PlayerStateIdle<PlayerStates>(PlayerStates.Walk, PlayerStates.Jump);
        PlayerStateWalk<PlayerStates> psWalk = new PlayerStateWalk<PlayerStates> (PlayerStates.Idle);
        PlayerStateJump<PlayerStates> psJump = new PlayerStateJump<PlayerStates>(PlayerStates.Idle, this);

        psIdle.AddTransition(PlayerStates.Walk, psWalk);
        psWalk.AddTransition(PlayerStates.Idle, psIdle);

        psJump.AddTransition(PlayerStates.Idle, psIdle);
        psIdle.AddTransition(PlayerStates.Jump, psJump);

        fsm.SetInit(psIdle);
    }

    void Update()
    {
        fsm.OnExecute();
    }
}
