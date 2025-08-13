using UnityEngine;

public class PlayerCollisions
{
    private PlayerController playerController;

    private Table auxiliarTable;
    private ClientView auxiliarClientView;


    public PlayerCollisions(PlayerController playerController)
    {
        this.playerController = playerController;
    }


    public void OnCollisionsEnter(Collision collision)
    {
        OnCollisionEnterWithAdministration(collision);
        //OnCollisionEnterWithTable(collision); 
    }

    public void OnCollisionsStay(Collision collision)
    {
        OnCollisionStayWithAdministrationAndLOS(collision);
        //OnCollisionStayWithTable(collision);
    }

    public void OnCollisionsExit(Collision collision)
    {
        OnCollisionExitWithAdministration(collision);
        //OnCollisionExitWithTable(collision);
    }

    public void OnTriggerEnter(Collider collider)
    {
        OnTriggerEnterWithPortalDungeon(collider);
    }


    /* ----------------------------------------COLLISION ENTER-------------------------------------------- */

    private void OnCollisionEnterWithAdministration(Collision collision)
    {
        if (collision.gameObject.CompareTag("Administration") && playerController.PlayerModel.IsLookingAtAdministration())
        {
            playerController.PlayerModel.IsCollidingAdministration = true;
            PlayerView.OnCollisionEnterWithAdministrationForAdministrationModeMessage?.Invoke();
        }
    }

    private void OnCollisionEnterWithTable(Collision collision)
    {
        if (collision.gameObject.CompareTag("Table"))
        {
            Table table = collision.gameObject.GetComponentInParent<Table>();

            if (table.IsDirty)
            {
                PlayerView.OnActivateSliderCleanDirtyTable?.Invoke();
                PlayerView.OnCollisionEnterWithTableForCleanDirtyTableMessage?.Invoke();
                return;
            }
        }
    }

    /* ------------------------------------------COLLISION STAY------------------------------------------- */


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

            if (table.IsDirty)
            {
                PlayerView.OnActivateSliderCleanDirtyTable?.Invoke();
                PlayerView.OnCollisionEnterWithTableForCleanDirtyTableMessage?.Invoke();

                if (PlayerInputs.Instance.CleanDirtyTable())
                {
                    PlayerController.OnCleanDirtyTableIncreaseSlider?.Invoke(table);
                    return;
                }

                else
                {
                    PlayerController.OnCleanDirtyTableDecreaseSlider?.Invoke(table);
                    return;
                }
            }

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
                PlayerView.OnCollisionExitWithTableForCleanDirtyTableMessage?.Invoke();

                auxiliarTable = null;
                auxiliarClientView = null;
            }
        }
    }

    /* -------------------------------------------COLLISION EXIT----------------------------------------- */
    
    private void OnCollisionExitWithAdministration(Collision collision)
    {
        if (collision.gameObject.CompareTag("Administration"))
        {
            playerController.PlayerModel.IsCollidingAdministration = false;
            PlayerView.OnCollisionExitWithAdministrationForAdministrationModeMessage?.Invoke();
        }
    }

    private void OnCollisionExitWithTable(Collision collision)
    {
        if (collision.gameObject.CompareTag("Table"))
        {
            Table table = collision.gameObject.GetComponentInParent<Table>();

            PlayerView.OnDeactivateSliderCleanDirtyTable?.Invoke();
            PlayerView.OnCollisionExitWithTableForCleanDirtyTableMessage?.Invoke();

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

    /* -------------------------------------------TRIGGER ENTER----------------------------------------- */

    private void OnTriggerEnterWithPortalDungeon(Collider collider)
    {
        if (collider.gameObject.CompareTag("PortalDungeon"))
        {
            collider.gameObject.SetActive(false); // Se desactiva el trigger porque sino ejecuta la corrutina varias veces
            string[] additiveScenes = { "DungeonUI", "CompartidoUI" };
            playerController.StartCoroutine(ScenesManager.Instance.LoadScene("Dungeon", additiveScenes));
        }
    }
}
