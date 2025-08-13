using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseManager : Singleton<PauseManager>
{
    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private AudioSource buttonSelected;

    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private List<GameObject> buttonsPause;

    private static event Action<List<GameObject>> onSendButtonsToEventSystem;

    private static event Action<GameObject> onSetSelectedCurrentGameObject;
    private static event Action onClearSelectedCurrentGameObject;

    private static event Action onRestoreSelectedGameObject; // Este evento es generico y sirve para todos los paneles de UI que esten abiertos cuando se pausa el juego

    private bool isGamePaused = false;

    public static Action<List<GameObject>> OnSendButtonsToEventSystem { get => onSendButtonsToEventSystem; set => onSendButtonsToEventSystem = value; }

    public static Action<GameObject> OnSetSelectedCurrentGameObject { get => onSetSelectedCurrentGameObject; set => onSetSelectedCurrentGameObject = value; }
    public static Action OnClearSelectedCurrentGameObject { get => onClearSelectedCurrentGameObject; set => onClearSelectedCurrentGameObject = value; }

    public static Action OnRestoreSelectedGameObject { get => onRestoreSelectedGameObject; set => onRestoreSelectedGameObject = value; }    

    public bool IsGamePaused { get => isGamePaused; }


    void Awake()
    {
        CreateSingleton(false);
        SuscribeToUpdateManagerEvent();
        InvokeEventToSendButtonsReferences();
    }

    // Simulacion de Update
    void UpdatePauseManager()
    {
        EnabledOrDisabledPausePanel();
    }

    void OnDestroy()
    {
        UnsuscribeToUpdateManagerEvent();
    }


    // Funcion asignada a botones en la UI para setear el selected GameObject del EventSystem con Mouse
    public void SetButtonAsSelectedGameObjectIfHasBeenHover(int indexButton)
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(buttonsPause[indexButton]);
        }
    }

    // Funcion asignada a botones en la UI para reproducir el sonido selected
    public void PlayAudioButtonSelectedWhenChangeSelectedGameObjectExceptFirstTime()
    {
        buttonSelected.Play();
    }

    // Funciones asignadas a botones de la UI
    public void ButtonResume()
    {
        buttonClick.Play();
        HidePause();
    }

    public void ButtonSettings()
    {
        buttonClick.Play();
        ShowSettings();
    }

    public void ButtonMainMenu()
    {
        buttonClick.Play();
        Time.timeScale = 1f;

        string[] additiveScenes = { "MainMenuUI" };
        StartCoroutine(loadSceneAfterSeconds("MainMenu", additiveScenes));
    }

    public void ButtonExit()
    {
        buttonClick.Play();
        StartCoroutine(ExitGameAfterSeconds());
    }

    public void ButtonBack()
    {
        buttonClick.Play();
        HideSettings();
    }


    private void SuscribeToUpdateManagerEvent()
    {
        UpdateManager.OnUpdate += UpdatePauseManager;
    }

    private void UnsuscribeToUpdateManagerEvent()
    {
        UpdateManager.OnUpdate -= UpdatePauseManager;
    }

    private void InvokeEventToSendButtonsReferences()
    {
        onSendButtonsToEventSystem?.Invoke(buttonsPause);
    }

    private void ShowPause()
    {
        onSetSelectedCurrentGameObject?.Invoke(buttonsPause[0]);
        Time.timeScale = 0f;
        isGamePaused = true;
        pausePanel.SetActive(true);
        DeviceManager.Instance.IsUIModeActive = true;
    }

    private void HidePause()
    {
        onClearSelectedCurrentGameObject?.Invoke();
        Time.timeScale = 1f;
        isGamePaused = false;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        DeviceManager.Instance.IsUIModeActive = false;

        onRestoreSelectedGameObject?.Invoke();
    }

    private void ShowSettings()
    {
        settingsPanel.SetActive(true);
    }

    private void HideSettings()
    {
        settingsPanel.SetActive(false);
    }

    private void EnabledOrDisabledPausePanel()
    {
        if (PlayerInputs.Instance.Pause())
        {
            buttonClick.Play();
            (isGamePaused ? (Action)HidePause : ShowPause)();
        }
    }

    private IEnumerator loadSceneAfterSeconds(string sceneName, string[] sceneNameAdditive)
    {
        yield return StartCoroutine(ScenesManager.Instance.LoadScene(sceneName, sceneNameAdditive));
    }

    private IEnumerator ExitGameAfterSeconds()
    {
        yield return StartCoroutine(ScenesManager.Instance.ExitGame());
    }
}
