using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AdministratingManager : MonoBehaviour
{
    [SerializeField] private GameObject rootGameObject; // GameObject padre con los botones hijos

    private Action onEnterAdmin, onExitAdmin;

    private List<Button> buttons;


    void Awake()
    {
        InitializeLambdaEvents();
        SuscribeToPlayerViewEvents();
    }

    void OnDestroy()
    {
        UnsuscribeToPlayerViewEvents();
    }


    // Funcion asignada a boton UI
    public void ButtonBuyIngredient(string ingredientName)
    {
        if (Enum.TryParse(ingredientName, out IngredientType ingredient))
        {
            int price = IngredientInventoryManager.Instance.GetPriceOfIngredient(ingredient);

            if (MoneyManager.Instance.CurrentMoney >= price && IngredientInventoryManager.Instance.HasUnlockIngredient(ingredient))
            {
                IngredientInventoryManager.Instance.IncreaseIngredientStock(ingredient);
                MoneyManager.Instance.SubMoney(price);
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
