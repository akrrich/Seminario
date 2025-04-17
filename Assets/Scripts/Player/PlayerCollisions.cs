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
            PlayerView.OnCollisionEnterWithOvenForCookModeMessage?.Invoke();
        }
    }

    public void OnCollisionEnterWithTable(Collision collision)
    {
        if (collision.gameObject.CompareTag("Table"))
        {
            bool hasChildren = false;

            foreach (Transform child in playerController.PlayerModel.Dish.transform)
            {
                if (child.childCount > 0)
                {
                    hasChildren = true;
                    break;
                }
            }

            if (hasChildren)
            {
                Table table = collision.gameObject.GetComponentInParent<Table>();

                if (table.IsOccupied)
                {
                    PlayerController.OnTableCollisionEnter?.Invoke(table);
                    PlayerView.OnCollisionEnterWithTableForHandOverMessage?.Invoke();
                }
            }
        }
    }

    public void OnCollisionEnterWithAdministration(Collision collision)
    {
        if (collision.gameObject.CompareTag("Administration") && playerController.PlayerModel.IsLookingAtAdministration())
        {
            playerController.PlayerModel.IsCollidingAdministration = true;
            PlayerView.OnCollisionEnterWithAdministrationForAdministrationModeMessage?.Invoke();
        }
    }

    /* ------------------------------------------STAY----------------------------------------------- */

    public void OnCollisionStayWithOvenAndLOS(Collision collision)
    {
        if (collision.gameObject.CompareTag("Oven") && playerController.PlayerModel.IsLookingAtOven())
        {
            playerController.PlayerModel.IsCollidingOven = true;
            PlayerView.OnCollisionEnterWithOvenForCookModeMessage?.Invoke();
        }

        if (collision.gameObject.CompareTag("Oven") && !playerController.PlayerModel.IsLookingAtOven())
        {
            PlayerView.OnCollisionExitWithOvenForCookModeMessage?.Invoke();
        }
    }

    public void OnCollisionStayWithAdministrationAndLOS(Collision collision)
    {
        if (collision.gameObject.CompareTag("Administration") && playerController.PlayerModel.IsLookingAtAdministration())
        {
            playerController.PlayerModel.IsCollidingAdministration = true;
            PlayerView.OnCollisionEnterWithAdministrationForAdministrationModeMessage?.Invoke();
        }

        if (collision.gameObject.CompareTag("Administration") && !playerController.PlayerModel.IsLookingAtAdministration())
        {
            PlayerView.OnCollisionExitWithAdministrationForAdministrationModeMessage?.Invoke();
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
            PlayerView.OnCollisionExitWithOvenForCookModeMessage?.Invoke();
        }
    }

    public void OnCollisionExitWithTable(Collision collision)
    {
        if (collision.gameObject.CompareTag("Table"))
        {
            PlayerController.OnTableCollisionExit?.Invoke();
            PlayerView.OnCollisionExitWithTableForHandOverMessage?.Invoke();
        }
    }

    public void OnCollisionExitWithAdministration(Collision collision)
    {
        if (collision.gameObject.CompareTag("Administration"))
        {
            playerController.PlayerModel.IsCollidingAdministration = false;
            PlayerView.OnCollisionExitWithAdministrationForAdministrationModeMessage?.Invoke();
        }
    }
}
