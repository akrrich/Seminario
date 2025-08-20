using System;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;

public class OpenTabernUI : MonoBehaviour
{
    [SerializeField] private GameObject panelOpenTabern;

    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private AudioSource buttonSelected;

    private List<GameObject> buttonsOpenTabern = new List<GameObject>();

    private GameObject lastSelectedButtonFromAdminPanel;

    private static event Action<GameObject> onSetSelectedCurrentGameObject;
    private static event Action onClearSelectedCurrentGameObject;
    
    private static event Action onOpenTabern;

    private bool ignoreFirstButtonSelected = true;

    public static Action<GameObject> OnSetSelectedCurrentGameObject { get => onSetSelectedCurrentGameObject; set => onSetSelectedCurrentGameObject = value; }
    public static Action OnClearSelectedCurrentGameObject { get => onClearSelectedCurrentGameObject; set => onClearSelectedCurrentGameObject = value; }

    public static Action OnOpenTabern { get => onOpenTabern; set => onOpenTabern = value; }


    void Awake()
    {
        SuscribeToUpdateManagerEvent();
        SuscribeToPauseManagerRestoreSelectedGameObjectEvent();
        GetComponents();
    }

    // Simulacion de Update
    void UpdateOpenTabernUI()
    {
        // Test
        if (Input.GetKeyDown(KeyCode.T) && !panelOpenTabern.gameObject.activeSelf)
        {
            DeviceManager.Instance.IsUIModeActive = true;

            onSetSelectedCurrentGameObject?.Invoke(buttonsOpenTabern[0]);
            panelOpenTabern.SetActive(true);
            return;
        }

        else if (Input.GetKeyDown(KeyCode.T) && panelOpenTabern.gameObject.activeSelf)
        {
            DeviceManager.Instance.IsUIModeActive = false;

            onClearSelectedCurrentGameObject?.Invoke();
            panelOpenTabern.SetActive(false);
            ignoreFirstButtonSelected = true;
            return;
        }

        CheckLastSelectedButtonIfAdminPanelIsOpen();
    }

    void OnDestroy()
    {
        UnsuscribeToUpdateManagerEvent();
        UnscribeToPauseManagerRestoreSelectedGameObjectEvent();
    }


    // Funcion asignada a botones de la UI
    public void ButtonOpenTabern()
    {
        buttonClick.Play();
        onOpenTabern?.Invoke();
    }

    // Funcion asignada a botones en la UI para setear el selected GameObject del EventSystem con Mouse
    public void SetButtonAsSelectedGameObjectIfHasBeenHover(int indexButton)
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(buttonsOpenTabern[indexButton]);
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


    private void SuscribeToUpdateManagerEvent()
    {
        UpdateManager.OnUpdate += UpdateOpenTabernUI;
    }

    private void UnsuscribeToUpdateManagerEvent()
    {
        UpdateManager.OnUpdate -= UpdateOpenTabernUI;
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
        foreach (Transform childs in panelOpenTabern.transform)
        {
            buttonsOpenTabern.Add(childs.gameObject);
        }
    }

    private void RestoreLastSelectedGameObjectIfGameWasPausedDuringAdministratingUI()
    {
        if (panelOpenTabern.activeSelf)
        {
            ignoreFirstButtonSelected = true;
            DeviceManager.Instance.IsUIModeActive = true;
            EventSystem.current.SetSelectedGameObject(lastSelectedButtonFromAdminPanel);
        }
    }

    private void CheckLastSelectedButtonIfAdminPanelIsOpen()
    {
        if (EventSystem.current != null && PauseManager.Instance != null && !PauseManager.Instance.IsGamePaused && panelOpenTabern.activeSelf)
        {
            lastSelectedButtonFromAdminPanel = EventSystem.current.currentSelectedGameObject;
        }
    }
}
