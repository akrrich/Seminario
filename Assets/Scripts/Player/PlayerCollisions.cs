using UnityEngine;

public class PlayerCollisions
{
    private PlayerController playerController;


    public PlayerCollisions(PlayerController playerController)
    {
        this.playerController = playerController;
    }


    public void OnCollisionEnterWithFloor(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            playerController.PlayerModel.IsGrounded = true;
        }
    }

    public void OnCollisionEnterWithOvenAndLOS(Collision collision)
    {
        if (collision.gameObject.CompareTag("Oven") && playerController.PlayerModel.IsLookingAtOven())
        {
            playerController.PlayerModel.IsCollidingOven = true;
            PlayerView.OnEnterInCookModeMessage?.Invoke();
        }
    }

    public void OnCollisionStayWithOvenAndLOS(Collision collision)
    {
        if (collision.gameObject.CompareTag("Oven") && playerController.PlayerModel.IsLookingAtOven())
        {
            playerController.PlayerModel.IsCollidingOven = true;
            PlayerView.OnEnterInCookModeMessage?.Invoke();
        }
    }

    public void OnCollisionExitWithFloor(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            playerController.PlayerModel.IsGrounded = false;
        }
    }

    public void OnCollisionExitWithOven(Collision colision)
    {
        if (colision.gameObject.CompareTag("Oven"))
        {
            playerController.PlayerModel.IsCollidingOven = false;
            PlayerView.OnExitInCookModeMessage?.Invoke();
        }
    }

    public void OnCollisionEnterWithItem(Collision colision)
    {
        if (colision.gameObject.CompareTag("Item"))
        {
            playerController.PlayerModel.IsCollidingItem = true;

            playerController.PlayerModel.CurrentItem = colision.gameObject;
        }
    }

    public void OnCollisionStayWithItem(Collision colision)
    {
        if (colision.gameObject.CompareTag("Item"))
        {
            playerController.PlayerModel.IsCollidingItem = true;

            playerController.PlayerModel.CurrentItem = colision.gameObject;
        }
    }

    public void OnCollisionExitWithItem(Collision colision)
    {
        if (colision.gameObject.CompareTag("Item"))
        {
            playerController.PlayerModel.IsCollidingItem = false;
        }
    }
}
