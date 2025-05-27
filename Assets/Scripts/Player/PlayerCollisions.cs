using UnityEngine;

public class PlayerCollisions
{
    private PlayerController playerController;

    private Table auxiliarTable;
    private ClientView auxiliarClientView;


    public void UpdateColls()
    {
        //Debug.Log("AuxiliarTable: " + auxiliarTable);
        //Debug.Log("AuxiliarClient: " + auxiliarClientView);
    }


    public PlayerCollisions(PlayerController playerController)
    {
        this.playerController = playerController;
    }


    public void OnCollisionsEnter(Collision collision)
    {
        OnCollisionEnterWithFloor(collision);
        OnCollisionEnterWithOvenAndLOS(collision);
        OnCollisionEnterWithTable(collision);
        OnCollisionEnterWithAdministration(collision);
    }

    public void OnCollisionsStay(Collision collision)
    {
        OnCollisionStayWithOvenAndLOS(collision);
        OnCollisionStayWithAdministrationAndLOS(collision);
        OnCollisionStayWithTable(collision);
    }

    public void OnCollisionsExit(Collision collision)
    {
        OnCollisionExitWithFloor(collision);
        OnCollisionExitWithOven(collision);
        OnCollisionExitWithTable(collision);
        OnCollisionExitWithAdministration(collision);
    }


    /* ----------------------------------------ENTER-------------------------------------------------- */

    private void OnCollisionEnterWithFloor(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            playerController.PlayerModel.IsGrounded = true;
        }
    }

    private void OnCollisionEnterWithOvenAndLOS(Collision collision)
    {
        if (collision.gameObject.CompareTag("Oven") && playerController.PlayerModel.IsLookingAtOven())
        {
            playerController.PlayerModel.IsCollidingOven = true;
            PlayerView.OnCollisionEnterWithOvenForCookModeMessage?.Invoke();
        }
    }

    private void OnCollisionEnterWithTable(Collision collision)
    {
        if (collision.gameObject.CompareTag("Table"))
        {
            Table table = collision.gameObject.GetComponentInParent<Table>();

            if (table.IsDirty) return;

            if (table.ChairPosition.childCount > 0) // Si tiene a alguien sentado
            {
                // Tomar Pedido
                ClientView clientView = table.gameObject.GetComponentInChildren<ClientView>();

                if (table.IsOccupied && clientView.ReturnSpriteWaitingToBeAttendedIsActive())
                {
                    clientView.CanTakeOrder = true;
                    PlayerController.OnTableCollisionEnterForTakeOrder?.Invoke(table);
                    PlayerView.OnCollisionEnterWithTableForTakeOrderMessage?.Invoke();
                    return;
                }

                // Entregar Pedido
                bool hasChildren = false;
                foreach (Transform child in playerController.PlayerView.Dish.transform)
                {
                    if (child.childCount > 0)
                    {
                        hasChildren = true;
                        break;
                    }
                }

                if (hasChildren)
                {
                    if (table.IsOccupied && clientView.ReturnSpriteFoodIsActive())
                    {
                        PlayerController.OnTableCollisionEnterForHandOverFood?.Invoke(table);
                        PlayerView.OnCollisionEnterWithTableForHandOverMessage?.Invoke();
                    }  
                }
            }
        }
    }

    private void OnCollisionEnterWithAdministration(Collision collision)
    {
        if (collision.gameObject.CompareTag("Administration") && playerController.PlayerModel.IsLookingAtAdministration())
        {
            playerController.PlayerModel.IsCollidingAdministration = true;
            PlayerView.OnCollisionEnterWithAdministrationForAdministrationModeMessage?.Invoke();
        }
    }

    /* ------------------------------------------STAY----------------------------------------------- */

    private void OnCollisionStayWithOvenAndLOS(Collision collision)
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

    private void OnCollisionStayWithAdministrationAndLOS(Collision collision)
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

    private void OnCollisionStayWithTable(Collision collision)
    {
        if (collision.gameObject.CompareTag("Table"))
        {
            Table table = collision.gameObject.GetComponentInParent<Table>();

            if (table.IsDirty) return;

            if (auxiliarTable == null || auxiliarTable != table)
            {
                auxiliarTable = table;
                auxiliarClientView = table.GetComponentInChildren<ClientView>();
            }

            // Si hay un cliente sentado
            if (table.ChairPosition.childCount > 0 && auxiliarClientView != null)
            {
                // Tomar pedido
                if (table.IsOccupied && auxiliarClientView.ReturnSpriteWaitingToBeAttendedIsActive())
                {
                    if (!auxiliarClientView.CanTakeOrder)
                    {
                        auxiliarClientView.CanTakeOrder = true;
                        PlayerController.OnTableCollisionEnterForTakeOrder?.Invoke(table);
                        PlayerView.OnCollisionEnterWithTableForTakeOrderMessage?.Invoke();
                    }
                }

                // Entregar pedido
                bool hasChildren = false;
                foreach (Transform child in playerController.PlayerView.Dish.transform)
                {
                    if (child.childCount > 0)
                    {
                        hasChildren = true;
                        break;
                    }
                }

                if (hasChildren && table.IsOccupied && auxiliarClientView.ReturnSpriteFoodIsActive())
                {
                    PlayerController.OnTableCollisionEnterForHandOverFood?.Invoke(table);
                    PlayerView.OnCollisionEnterWithTableForHandOverMessage?.Invoke();
                }
            }

            // Si no hay un cliente sentado
            else
            {
                // El cliente se fue
                if (auxiliarClientView != null)
                {
                    auxiliarClientView.CanTakeOrder = false;
                }

                PlayerController.OnTableCollisionExitForTakeOrder?.Invoke();
                PlayerController.OnTableCollisionExitForHandOverFood?.Invoke();
                PlayerView.OnCollisionExitWithTableForTakeOrderMessage?.Invoke();
                PlayerView.OnCollisionExitWithTableForHandOverMessage?.Invoke();

                auxiliarTable = null;
                auxiliarClientView = null;
            }
        }
    }

    /* -------------------------------------------EXIT--------------------------------------------- */

    private void OnCollisionExitWithFloor(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            playerController.PlayerModel.IsGrounded = false;
        }
    }

    private void OnCollisionExitWithOven(Collision collision)
    {
        if (collision.gameObject.CompareTag("Oven"))
        {
            playerController.PlayerModel.IsCollidingOven = false;
            PlayerView.OnCollisionExitWithOvenForCookModeMessage?.Invoke();
        }
    }

    private void OnCollisionExitWithTable(Collision collision)
    {
        if (collision.gameObject.CompareTag("Table"))
        {
            Table table = collision.gameObject.GetComponentInParent<Table>();

            if (table.ChairPosition.childCount > 0) // Si tiene a alguien sentado
            {
                ClientView clientView = table.gameObject.GetComponentInChildren<ClientView>();
                clientView.CanTakeOrder = false;

                PlayerController.OnTableCollisionExitForTakeOrder?.Invoke();
                PlayerController.OnTableCollisionExitForHandOverFood?.Invoke();
                PlayerView.OnCollisionExitWithTableForTakeOrderMessage?.Invoke();
                PlayerView.OnCollisionExitWithTableForHandOverMessage?.Invoke();

                auxiliarTable = null;
                auxiliarClientView = null;
                return;
            }
        }
    }

    private void OnCollisionExitWithAdministration(Collision collision)
    {
        if (collision.gameObject.CompareTag("Administration"))
        {
            playerController.PlayerModel.IsCollidingAdministration = false;
            PlayerView.OnCollisionExitWithAdministrationForAdministrationModeMessage?.Invoke();
        }
    }
}
