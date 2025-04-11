using UnityEngine;

public class PlayerCollisions
{
    private PlayerController playerController;


    public PlayerCollisions(PlayerController playerController)
    {
        this.playerController = playerController;
    }


    /* ----------------------------------------ENTER-------------------------------------------------- */

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

    public void OnCollisionEnterWithItem(Collision collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            playerController.PlayerModel.IsCollidingItem = true;

            playerController.PlayerModel.CurrentItem = collision.gameObject;
        }
    }

    public void OnCollisionEnterWithTable(Collision collision)
    {
        if (collision.gameObject.CompareTag("Table"))
        {
            bool hasGrandchildren = false;

            foreach (Transform child in playerController.PlayerModel.Dish.transform)
            {
                if (child.childCount > 0)
                {
                    hasGrandchildren = true;
                    break;
                }
            }

            if (hasGrandchildren)
            {
                Table table = collision.gameObject.GetComponentInParent<Table>();

                PlayerController.OnTableCollisionEnter?.Invoke(table);
            }
        }
    }


    /* ------------------------------------------STAY----------------------------------------------- */

    public void OnCollisionStayWithOvenAndLOS(Collision collision)
    {
        if (collision.gameObject.CompareTag("Oven") && playerController.PlayerModel.IsLookingAtOven())
        {
            playerController.PlayerModel.IsCollidingOven = true;
            PlayerView.OnEnterInCookModeMessage?.Invoke();
        }
    }

    public void OnCollisionStayWithItem(Collision collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            playerController.PlayerModel.IsCollidingItem = true;

            playerController.PlayerModel.CurrentItem = collision.gameObject;
        }
    }


    /* -------------------------------------------EXIT--------------------------------------------- */

    public void OnCollisionExitWithFloor(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            playerController.PlayerModel.IsGrounded = false;
        }
    }

    public void OnCollisionExitWithOven(Collision collision)
    {
        if (collision.gameObject.CompareTag("Oven"))
        {
            playerController.PlayerModel.IsCollidingOven = false;
            PlayerView.OnExitInCookModeMessage?.Invoke();
        }
    }

    public void OnCollisionExitWithItem(Collision collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            playerController.PlayerModel.IsCollidingItem = false;
        }
    }

    public void OnCollisionExitWithTable(Collision collision)
    {
        if (collision.gameObject.CompareTag("Table"))
        {
            PlayerController.OnTableCollisionExit?.Invoke();
        }
    }
}
