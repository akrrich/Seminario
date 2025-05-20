using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private List<GameObject> buttonsMainMenu;

    private AudioSource buttonClick;

    private static event Action<List<GameObject>> onSendButtonsToEventSystem;

    public static Action<List<GameObject>> OnSendButtonsToEventSystem { get => onSendButtonsToEventSystem; set => onSendButtonsToEventSystem = value; }


    void Awake()
    {
        InvokeEventToSendButtonsReferences();
        GetComponents();
    }


    // Funcion asignada a botones en la UI para setear el selected GameObject del EventSystem
    public void SetButtonAsSelectedGameObjectIfHasBeenHover(int indexButton)
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(buttonsMainMenu[indexButton]);
        }
    }

    // Funcion asignada a boton en la UI
    public void ButtonPlay()
    {
        DeviceManager.Instance.IsUIActive = false;
        StartCoroutine(LoadSceneAfterButtonClick());
    }

    // Funcion asignada a boton en la UI
    public void ButtonExit()
    {
        DeviceManager.Instance.IsUIActive = false;
        StartCoroutine(CloseGameAfterClickButton());
    }


    private void InvokeEventToSendButtonsReferences()
    {
        onSendButtonsToEventSystem?.Invoke(buttonsMainMenu);
    }

    private void GetComponents()
    {
        buttonClick = GetComponent<AudioSource>();
    }

    private IEnumerator LoadSceneAfterButtonClick()
    {
        buttonClick.Play();

        string[] additiveScenes = { "TabernUI", "CompartidoUI" };
        yield return StartCoroutine(ScenesManager.Instance.LoadScene("Tabern", additiveScenes));
    }

    private IEnumerator CloseGameAfterClickButton()
    {
        buttonClick.Play();

        yield return new WaitForSeconds(buttonClick.clip.length);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
