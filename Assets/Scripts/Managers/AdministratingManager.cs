using UnityEngine;

public class AdministratingManager : MonoBehaviour
{
    [SerializeField] private GameObject rootGameObject; // GameObject padre con los botones hijos


    void Awake()
    {
        SuscribeToPlayerViewEvents();
    }

    void OnDestroy()
    {
        UnsuscribeToPlayerViewEvents();
    }


    // Funcion asignada a un boton
    public void ButtonBuyFood()
    {

    }


    private void SuscribeToPlayerViewEvents()
    {
        PlayerView.OnEnterInAdministrationMode += () => ActiveOrDeactivateRootGameObject(true);
        PlayerView.OnExitInAdministrationMode += () => ActiveOrDeactivateRootGameObject(false);
    }

    private void UnsuscribeToPlayerViewEvents()
    {
        PlayerView.OnEnterInAdministrationMode -= () => ActiveOrDeactivateRootGameObject(true);
        PlayerView.OnExitInAdministrationMode -= () => ActiveOrDeactivateRootGameObject(false);
    }

    private void ActiveOrDeactivateRootGameObject(bool state)
    {
        rootGameObject.SetActive(state);
    }
}
