using UnityEngine;

public class PlayerCollisions
{
    private PlayerController playerController;


    public PlayerCollisions(PlayerController playerController)
    {
        this.playerController = playerController;
    }


    public void OnCollisionsEnter(Collision collision)
    {
        OnCollisionEnterWithAdministration(collision);
    }

    public void OnCollisionsStay(Collision collision)
    {
        OnCollisionStayWithAdministrationAndLOS(collision);
    }

    public void OnCollisionsExit(Collision collision)
    {
        OnCollisionExitWithAdministration(collision);
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

    /* -------------------------------------------COLLISION EXIT----------------------------------------- */
    
    private void OnCollisionExitWithAdministration(Collision collision)
    {
        if (collision.gameObject.CompareTag("Administration"))
        {
            playerController.PlayerModel.IsCollidingAdministration = false;
            PlayerView.OnCollisionExitWithAdministrationForAdministrationModeMessage?.Invoke();
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
