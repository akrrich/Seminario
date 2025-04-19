using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    private static PauseManager instance;

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
        HidePause();
    }

    public void ButtonSettings()
    {
        ShowSettings();
    }

    public void ButtonMainMenu()
    {
        loadSceneAfterSeconds("MainMenu");
    }

    public void ButtonExit()
    {
        ExitGameAfterSeconds();
    }

    public void ButtonBack()
    {
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
            (isGamePaused ? (Action)HidePause : ShowPause)();
        }
    }

    private IEnumerator loadSceneAfterSeconds(string sceneName)
    {
        yield return new WaitForSeconds(2);

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator ExitGameAfterSeconds()
    {
        yield return new WaitForSeconds(2);

        Application.Quit();
    }
}
