using UnityEngine;

public class PlayerStateJump<T> : State<T>
{
    private PlayerController playerController;

    private T inputToIdle;

    private float jumpForce = 5;


    public PlayerStateJump(T inputToIdle, PlayerController playerController)
    {
        this.inputToIdle = inputToIdle;
        this.playerController = playerController;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Jump");

        playerController.Rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public override void Execute()
    {
        base.Execute();


        // Agregar logica de cuando toca el suelo
    }
}
