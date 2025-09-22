using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private List<GameObject> buttonsMainMenu;

    [SerializeField] private GameObject panelSettings;

    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private AudioSource buttonSelected;

    private static event Action<List<GameObject>> onSendButtonsToEventSystem;
    private static event Action onButtonSettingsClickToShowCorrectPanel;

    private bool ignoreFirstButtonSelected = true;

    public static Action<List<GameObject>> OnSendButtonsToEventSystem { get => onSendButtonsToEventSystem; set => onSendButtonsToEventSystem = value; }
    public static Action OnButtonSettingsClickToShowCorrectPanel { get => onButtonSettingsClickToShowCorrectPanel; set => onButtonSettingsClickToShowCorrectPanel = value; }


    void Awake()
    {
        InvokeEventToSendButtonsReferences();
    }


    // Funcion asignada a botones en la UI para setear el selected GameObject del EventSystem con Mouse
    public void SetButtonAsSelectedGameObjectIfHasBeenHover(int indexButton)
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(buttonsMainMenu[indexButton]);
        }
    }

    // Funcion asignada a botones en la UI para reproducir el sonido selected excepto la primero vez
    public void PlayAudioButtonSelectedWhenChangeSelectedGameObjectExceptFirstTime()
    {
        if (!ignoreFirstButtonSelected)
        {
            buttonSelected.Play();
            return;
        }

        ignoreFirstButtonSelected = false;
    }

    // Funcion asignada a boton en la UI
    public void ButtonNewGame()
    {
        SaveSystemManager.DeleteAllData();
        GameManager.Instance.GameSessionType = GameSessionType.New;
        GameManager.Instance.OnGameSessionStarted?.Invoke();
        DeviceManager.Instance.IsUIModeActive = false;
        StartCoroutine(LoadSceneAfterButtonClick());
    }

    // Funcion asignada a boton en la UI
    public void ButtonLoadGame()
    {
        GameManager.Instance.GameSessionType = GameSessionType.Load;
        GameManager.Instance.OnGameSessionStarted?.Invoke();
        DeviceManager.Instance.IsUIModeActive = false;
        StartCoroutine(LoadSceneAfterButtonClick());
    }

    // Funcion asignada a boton en la UI
    public void ButtonSettings()
    {
        buttonClick.Play();

        foreach (var button in buttonsMainMenu)
        {
            button.SetActive(false);
        }

        panelSettings.SetActive(true);
        onButtonSettingsClickToShowCorrectPanel?.Invoke();
    }

    // Funcion asignada a boton en la UI
    public void ButtonCredits()
    {
        buttonClick.Play();
    }

    // Funcion asignada a boton en la UI
    public void ButtonExit()
    {
        DeviceManager.Instance.IsUIModeActive = false;
        StartCoroutine(CloseGameAfterClickButton());
    }

    // Funcion asignada a boton en la UI
    public void ButtonBack()
    {
        buttonClick.Play();

        foreach (var button in buttonsMainMenu)
        {
            button.SetActive(true);
        }

        panelSettings.SetActive(false);
        EventSystem.current.SetSelectedGameObject(buttonsMainMenu[2]);
    }


    private void InvokeEventToSendButtonsReferences()
    {
        onSendButtonsToEventSystem?.Invoke(buttonsMainMenu);
    }

    private IEnumerator LoadSceneAfterButtonClick()
    {
        buttonClick.Play();

        if (GameManager.Instance.GameSessionType == GameSessionType.Load && SaveSystemManager.SaveExists())
        {
            SaveData data = SaveSystemManager.LoadGame();
            string[] additiveScenes = { data.lastSceneName + "UI", "CompartidoUI" };
            yield return StartCoroutine(ScenesManager.Instance.LoadScene(data.lastSceneName, additiveScenes));
        }

        else
        {
            string[] additiveScenes = { "TabernUI", "CompartidoUI" };
            yield return StartCoroutine(ScenesManager.Instance.LoadScene("Tabern", additiveScenes));
        }
    }

    private IEnumerator CloseGameAfterClickButton()
    {
        yield return StartCoroutine(ScenesManager.Instance.ExitGame());
    }
}
