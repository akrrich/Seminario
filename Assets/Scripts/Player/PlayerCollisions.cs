using UnityEngine;

public class PlayerCollisions
{
    private PlayerController playerController;

    private Table auxiliarTable;
    private ClientView auxiliarClientView;


    // Test
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
        OnCollisionEnterWithCookingDeskUIAndLOS(collision);
        OnCollisionEnterWithTrashAndLOS(collision);
        OnCollisionEnterWithAdministration(collision);
        OnCollisionEnterWithTable(collision); 
    }

    public void OnCollisionsStay(Collision collision)
    {
        OnCollisionStayWithCookingDeskUIAndLOS(collision);
        OnCollisionStayWithTrashAndLOS(collision);
        OnCollisionStayWithAdministrationAndLOS(collision);
        OnCollisionStayWithTable(collision);
    }

    public void OnCollisionsExit(Collision collision)
    {
        OnCollisionExitWithFloor(collision);
        OnCollisionExitWithCookingDeskUI(collision);
        OnCollisionExitWithTrash(collision);
        OnCollisionExitWithAdministration(collision);
        OnCollisionExitWithTable(collision);
    }

    public void OnTriggerEnter(Collider collider)
    {
        OnTriggerEnterWithPortalDungeon(collider);
    }


    /* ----------------------------------------COLLISION ENTER-------------------------------------------- */

    private void OnCollisionEnterWithFloor(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            playerController.PlayerModel.IsGrounded = true;
        }
    }

    private void OnCollisionEnterWithCookingDeskUIAndLOS(Collision collision)
    {
        if (collision.gameObject.CompareTag("CookingDeskUI") && playerController.PlayerModel.IsLookingAtCookingDeskUI())
        {
            playerController.PlayerModel.IsCollidingCookingDeskUI = true;
            PlayerView.OnCollisionEnterWithCookingDeskUIForCookModeMessage?.Invoke();
        }
    }

    private void OnCollisionEnterWithTrashAndLOS(Collision collision)
    {
        if (collision.gameObject.CompareTag("Trash") && playerController.PlayerModel.IsLookingAtTrash(playerController))
        {
            playerController.PlayerModel.IsCollidingTrash = true;
            PlayerView.OnCollisionEnterWithTrashForTrashModeMessage?.Invoke();   
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

            /*if (table.ChairPosition.childCount > 0) // Si tiene a alguien sentado
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
            }*/
        }
    }

    /* ------------------------------------------COLLISION STAY------------------------------------------- */

    private void OnCollisionStayWithCookingDeskUIAndLOS(Collision collision)
    {
        if (collision.gameObject.CompareTag("CookingDeskUI") && playerController.PlayerModel.IsLookingAtCookingDeskUI())
        {
            playerController.PlayerModel.IsCollidingCookingDeskUI = true;
            PlayerView.OnCollisionEnterWithCookingDeskUIForCookModeMessage?.Invoke();
        }

        if (collision.gameObject.CompareTag("CookingDeskUI") && !playerController.PlayerModel.IsLookingAtCookingDeskUI())
        {
            PlayerView.OnCollisionExitWithCookingDeskUIForCookModeMessage?.Invoke();
        }
    }

    private void OnCollisionStayWithTrashAndLOS(Collision collision)
    {
        if (collision.gameObject.CompareTag("Trash") && playerController.PlayerModel.IsLookingAtTrash(playerController))
        {
            playerController.PlayerModel.IsCollidingTrash = true;
            PlayerView.OnCollisionEnterWithTrashForTrashModeMessage?.Invoke();
        }

        if (collision.gameObject.CompareTag("Trash") && !playerController.PlayerModel.IsLookingAtTrash(playerController))
        {
            PlayerView.OnCollisionExitWithTrashForTrashModeMessage?.Invoke();
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

            if (table.IsDirty)
            {
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

    private void OnCollisionExitWithFloor(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            playerController.PlayerModel.IsGrounded = false;
        }
    }

    private void OnCollisionExitWithCookingDeskUI(Collision collision)
    {
        if (collision.gameObject.CompareTag("CookingDeskUI"))
        {
            playerController.PlayerModel.IsCollidingCookingDeskUI = false;
            PlayerView.OnCollisionExitWithCookingDeskUIForCookModeMessage?.Invoke();
        }
    }

    private void OnCollisionExitWithTrash(Collision collision)
    {
        if (collision.gameObject.CompareTag("Trash"))
        {
            playerController.PlayerModel.IsCollidingTrash = false;
            PlayerView.OnCollisionExitWithTrashForTrashModeMessage?.Invoke();
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
