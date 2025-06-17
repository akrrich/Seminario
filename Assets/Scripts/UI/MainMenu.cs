using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private List<GameObject> buttonsMainMenu;

    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private AudioSource buttonSelected;

    private static event Action<List<GameObject>> onSendButtonsToEventSystem;

    private bool ignoreFirstButtonSelected = true;

    public static Action<List<GameObject>> OnSendButtonsToEventSystem { get => onSendButtonsToEventSystem; set => onSendButtonsToEventSystem = value; }


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
    public void ButtonPlay()
    {
        DeviceManager.Instance.IsUIModeActive = false;
        StartCoroutine(LoadSceneAfterButtonClick());
    }

    // Funcion asignada a boton en la UI
    public void ButtonExit()
    {
        DeviceManager.Instance.IsUIModeActive = false;
        StartCoroutine(CloseGameAfterClickButton());
    }


    private void InvokeEventToSendButtonsReferences()
    {
        onSendButtonsToEventSystem?.Invoke(buttonsMainMenu);
    }

    private IEnumerator LoadSceneAfterButtonClick()
    {
        buttonClick.Play();

        string[] additiveScenes = { "TabernUI", "CompartidoUI" };
        yield return StartCoroutine(ScenesManager.Instance.LoadScene("Tabern", additiveScenes));
    }

    private IEnumerator CloseGameAfterClickButton()
    {
        yield return StartCoroutine(ScenesManager.Instance.ExitGame());
    }
}
