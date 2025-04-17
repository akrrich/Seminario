using System;
using UnityEngine;

public class AdministratingManager : MonoBehaviour
{
    [SerializeField] private GameObject rootGameObject; // GameObject padre con los botones hijos

    private Action onEnterAdmin, onExitAdmin;


    void Awake()
    {
        InitializeLambdaEvents();
        SuscribeToPlayerViewEvents();
    }

    void OnDestroy()
    {
        UnsuscribeToPlayerViewEvents();
    }


    // Funcion asignada a un boton
    public void ButtonBuyFood(string foodTypeName)
    {
        if (Enum.TryParse(foodTypeName, out FoodType foodType))
        {
            int priceFood = InventoryFoodManager.Instance.GetPriceOfFood(foodType);

            if (MoneyManager.Instance.CurrentMoney >= priceFood)
            {
                InventoryFoodManager.Instance.BuyFood(foodType);

                MoneyManager.Instance.SubMoney(priceFood);
            }
        }
    }


    private void InitializeLambdaEvents()
    {
        onEnterAdmin += () => ActiveOrDeactivateRootGameObject(true);
        onExitAdmin += () => ActiveOrDeactivateRootGameObject(false);
    }

    private void SuscribeToPlayerViewEvents()
    {
        PlayerView.OnEnterInAdministrationMode += onEnterAdmin;
        PlayerView.OnExitInAdministrationMode += onExitAdmin;
    }

    private void UnsuscribeToPlayerViewEvents()
    {
        PlayerView.OnEnterInAdministrationMode -= onEnterAdmin;
        PlayerView.OnExitInAdministrationMode -= onExitAdmin;
    }

    private void ActiveOrDeactivateRootGameObject(bool state)
    {
        rootGameObject.SetActive(state);
    }
}
