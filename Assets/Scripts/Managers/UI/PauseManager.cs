using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    private static PauseManager instance;

    private AudioSource buttonClick;

    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    private bool isGamePaused = false;

    public static PauseManager Instance { get => instance; }

    public bool IsGamePaused { get => isGamePaused; }


    void Awake()
    {
        CreateSingleton();
        GetComponents();
    }

    void Update()
    {
        EnabledOrDisabledPausePanel();
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
        StartCoroutine(loadSceneAfterSeconds("MainMenu", "MainMenuUI"));
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


    private void CreateSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void GetComponents()
    {
        buttonClick = GetComponent<AudioSource>();
    }


    private void ShowPause()
    {
        Time.timeScale = 0f;
        isGamePaused = true;
        pausePanel.SetActive(true);
    }

    private void HidePause()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
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

    private IEnumerator loadSceneAfterSeconds(string sceneName, string sceneNameAdditive)
    {
        yield return new WaitForSeconds(buttonClick.clip.length);

        ScenesManager.Instance.LoadScene(sceneName);
        ScenesManager.Instance.LoadScene(sceneNameAdditive, LoadSceneMode.Additive);
    }

    private IEnumerator ExitGameAfterSeconds()
    {
        yield return new WaitForSeconds(buttonClick.clip.length);

        Application.Quit();
    }
}
