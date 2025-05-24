using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CookingManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject rootGameObject; // GameObject padre con los botones de hijos

    /// <summary>
    /// Agregar ruido de cancelacion si no tiene ingredientes para cocinar una receta
    /// </summary>
    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private AudioSource buttonSelected;

    private List<GameObject> buttonsCooking  = new List<GameObject>();

    private GameObject lastSelectedButtonFromCookingPanel;

    private static event Action<string> onButtonGetFood;

    private event Action onEnterCook, onExitCook;

    private static event Action<List<GameObject>> onSendButtonsToEventSystem;

    private static event Action<GameObject> onSetSelectedCurrentGameObject;
    private static event Action onClearSelectedCurrentGameObject;

    private bool ignoreFirstButtonSelected = true;

    public static Action<string> OnButtonSetFood { get => onButtonGetFood; set => onButtonGetFood = value; }

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
        UnSuscribeToPlayerViewEvents();
        UnscribeToPauseManagerRestoreSelectedGameObjectEvent();
    }


    // Funcion asignada a botones en la UI para setear el selected GameObject del EventSystem con Mouse
    public void SetButtonAsSelectedGameObjectIfHasBeenHover(int indexButton)
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(buttonsCooking[indexButton]);
        }
    }

    // Funcion asignada a botones en la UI para reproducir el sonido selected
    public void PlayAudioButtonSelectedWhenChangeSelectedGameObjectExceptFirstTime()
    {
        if (!ignoreFirstButtonSelected)
        {
            buttonClick.Play();
            return;
        }

        ignoreFirstButtonSelected = false;
    }

    // Funcion asignada a los botones de la UI
    public void ButtonGetFood(string foodName)
    {
        buttonSelected.Play();
        onButtonGetFood?.Invoke(foodName);
    }


    private void InitializeLambdaEvents()
    {
        onEnterCook += () => ActiveOrDeactivateRootGameObject(true);
        onExitCook += () => ActiveOrDeactivateRootGameObject(false);
    }

    private void SuscribeToPlayerViewEvents()
    {
        PlayerView.OnEnterInCookMode += onEnterCook;
        PlayerView.OnExitInCookMode += onExitCook;
    }

    private void UnSuscribeToPlayerViewEvents()
    {
        PlayerView.OnEnterInCookMode -= onEnterCook;
        PlayerView.OnExitInCookMode -= onExitCook;
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
            buttonsCooking.Add(childs.gameObject);
        }
    }

    private void InvokeEventToSendButtonsReferences()
    {
        onSendButtonsToEventSystem?.Invoke(buttonsCooking);
    }

    private void ActiveOrDeactivateRootGameObject(bool state)
    {
        rootGameObject.SetActive(state);

        if (state)
        {
            DeviceManager.Instance.IsUIModeActive = true;
            onSetSelectedCurrentGameObject?.Invoke(buttonsCooking[0]);
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
            EventSystem.current.SetSelectedGameObject(lastSelectedButtonFromCookingPanel);
        }
    }

    private void CheckLastSelectedButtonIfAdminPanelIsOpen()
    {
        if (EventSystem.current != null && PauseManager.Instance != null && !PauseManager.Instance.IsGamePaused && rootGameObject.activeSelf)
        {
            lastSelectedButtonFromCookingPanel = EventSystem.current.currentSelectedGameObject;
        }
    }
}
