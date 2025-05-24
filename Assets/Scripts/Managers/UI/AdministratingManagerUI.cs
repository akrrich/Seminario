using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class AdministratingManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject rootGameObject; // GameObject padre con los botones hijos

    /// <summary>
    /// Agregar que sonido de cancelacion si no puede comprar el ingrediente
    /// </summary>
    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private AudioSource buttonSelected;

    private List<GameObject> buttonsAdministrating = new List<GameObject>();

    private GameObject lastSelectedButtonFromAdminPanel;

    private event Action onEnterAdmin, onExitAdmin;

    private static event Action<List<GameObject>> onSendButtonsToEventSystem;

    private static event Action<GameObject> onSetSelectedCurrentGameObject;
    private static event Action onClearSelectedCurrentGameObject;

    private bool ignoreFirstButtonSelected = true;

    public static Action<List<GameObject>> OnSendButtonsToEventSystem { get => onSendButtonsToEventSystem; set => onSendButtonsToEventSystem = value; }

    public static Action<GameObject> OnSetSelectedCurrentGameObject { get => onSetSelectedCurrentGameObject; set => onSetSelectedCurrentGameObject = value; }
    public static Action OnClearSelectedCurrentGameObject { get => onClearSelectedCurrentGameObject; set => onClearSelectedCurrentGameObject = value; }


    void Awake()
    {
        InitializeLambdaEvents();
        SuscribeToPlayerViewEvents();
        SuscribeToPauseManagerRestoreSelectedGameObjectEvent();
        GetComponents();
        InvokeEventToSendButtonsReferences();
    }

    void Update()
    {
        CheckLastSelectedButtonIfAdminPanelIsOpen();
    }

    void OnDestroy()
    {
        UnsuscribeToPlayerViewEvents();
        UnscribeToPauseManagerRestoreSelectedGameObjectEvent();
    }


    // Funcion asignada a botones en la UI para setear el selected GameObject del EventSystem con Mouse
    public void SetButtonAsSelectedGameObjectIfHasBeenHover(int indexButton)
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(buttonsAdministrating[indexButton]);
        }
    }

    // Funcion asignada a botones en la UI para reproducir el sonido selected
    public void PlayAudioButtonSelectedWhenChangeSelectedGameObjectExceptFirstTime()
    {
        if (!ignoreFirstButtonSelected)
        {
            buttonSelected.Play();
            return;
        }

        ignoreFirstButtonSelected = false;
    }

    // Funcion asignada a boton UI
    public void ButtonBuyIngredient(string ingredientName)
    {
        if (Enum.TryParse(ingredientName, out IngredientType ingredient))
        {
            int price = IngredientInventoryManager.Instance.GetPriceOfIngredient(ingredient);

            if (MoneyManager.Instance.CurrentMoney >= price)
            {
                buttonClick.Play();
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

    private void SuscribeToPauseManagerRestoreSelectedGameObjectEvent()
    {
        PauseManager.OnRestoreSelectedGameObject += RestoreLastSelectedGameObjectIfGameWasPausedDuringAdministratingUI;
    }

    private void UnscribeToPauseManagerRestoreSelectedGameObjectEvent()
    {
        PauseManager.OnRestoreSelectedGameObject -= RestoreLastSelectedGameObjectIfGameWasPausedDuringAdministratingUI;
    }

    private void GetComponents()
    {
        foreach (Transform childs in rootGameObject.transform)
        {
            buttonsAdministrating.Add(childs.gameObject);
        }
    }

    private void InvokeEventToSendButtonsReferences()
    {
        onSendButtonsToEventSystem?.Invoke(buttonsAdministrating);
    }

    private void ActiveOrDeactivateRootGameObject(bool state)
    {
        rootGameObject.SetActive(state);

        if (state)
        {
            DeviceManager.Instance.IsUIModeActive = true;
            onSetSelectedCurrentGameObject?.Invoke(buttonsAdministrating[0]);
        }

        else
        {
            ignoreFirstButtonSelected = true;
            DeviceManager.Instance.IsUIModeActive = false;
            onClearSelectedCurrentGameObject?.Invoke();
        }
    }

    private void RestoreLastSelectedGameObjectIfGameWasPausedDuringAdministratingUI()
    {
        if (rootGameObject.activeSelf)
        {
            ignoreFirstButtonSelected = true;
            DeviceManager.Instance.IsUIModeActive = true;
            EventSystem.current.SetSelectedGameObject(lastSelectedButtonFromAdminPanel);
        }
    }

    private void CheckLastSelectedButtonIfAdminPanelIsOpen()
    {
        if (EventSystem.current != null && PauseManager.Instance != null && !PauseManager.Instance.IsGamePaused && rootGameObject.activeSelf)
        {
            lastSelectedButtonFromAdminPanel = EventSystem.current.currentSelectedGameObject;
        }
    }
}
