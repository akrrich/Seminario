using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : Singleton<ScenesManager>
{
    /// <summary>
    /// Con un update manager se soluciona que se pueda seguir interactuando mientras esta en pantalla de carga, por ejemplo para que no pueda poner pausa
    /// </summary>

    /// <sumary>
    /// Agregar metodo futuro para cerrar el juego con pantalla de carga y guardar los datos en ese tiempo
    /// </sumary>

    [SerializeField] private ScenesManagerData scenesManagerData;

    [SerializeField] private GameObject loadingScenePanel;
    [SerializeField] private GameObject exitGamePanel;

#pragma warning disable 0414
    /// <summary>
    /// Esto sirve para el UpdateManager, pero por ahora no srive de NADA
    /// </summary>
    private bool isInLoadingScenePanel = false; 
    private bool isInExitGamePanel = false;
#pragma warning restore 0414


    void Awake()
    {
        CreateSingleton(true);
        DontDestroyOnLoadPanels();
        SetInitializedScene();
    }


    // Para pasar de una escena a otra con pantalla de carga
    public IEnumerator LoadScene(string sceneName, string[] additiveScenes)
    {
        loadingScenePanel.SetActive(true);
        isInLoadingScenePanel = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        float elapsedTime = 0f;

        while (!asyncLoad.isDone)
        {
            elapsedTime += Time.deltaTime;

            if (asyncLoad.progress >= 0.9f && elapsedTime >= scenesManagerData.DuringTimeLoadingScenePanel)
            {
                if (additiveScenes != null)
                {
                    for (int i = 0; i < additiveScenes.Length; i++)
                    {
                        AsyncOperation additiveLoad = LoadSceneAdditive(additiveScenes[i]);
                    }
                }

                StartCoroutine(DisableLoadingScenePanelAfterSeconds());

                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    // Para cerrar el juego con pantalla de carga
    public IEnumerator ExitGame()
    {
        exitGamePanel.SetActive(true);
        isInExitGamePanel = true;

        yield return new WaitForSecondsRealtime(scenesManagerData.DuringTimeExitGamePanel);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }


    // Esto sirve para que una vez cargada la nueva escena, espere 3 segundos para desactivar el panel, para que permita cargar Awake y Start de la nueva escena cargada
    private IEnumerator DisableLoadingScenePanelAfterSeconds()
    {
        yield return new WaitForSeconds(3);

        isInLoadingScenePanel = false;
        loadingScenePanel.SetActive(false);
    }

    private void DontDestroyOnLoadPanels()
    {
        if (loadingScenePanel != null)
        {
            DontDestroyOnLoad(loadingScenePanel.transform.root.gameObject);
        }

        if (exitGamePanel != null)
        {
            DontDestroyOnLoad(exitGamePanel.transform.root.gameObject);
        }
    }

    // Este metodo solamente funciona para cuando se inicia el programa, es decir solamente una vez en toda la ejecucion
    private void SetInitializedScene()
    {
        Scene initializedCurrentScene = SceneManager.GetActiveScene();

        switch (initializedCurrentScene.name)
        {
            case "MainMenu":
                DeviceManager.Instance.IsUIModeActive = true;
                LoadSceneAdditive("MainMenuUI");
                break;

            case "TavernAssets":
                DeviceManager.Instance.IsUIModeActive = false;
                LoadSceneAdditive("TabernUI");
                LoadSceneAdditive("CompartidoUI");
                break;

            case "Dungeon":
                DeviceManager.Instance.IsUIModeActive = false;
                LoadSceneAdditive("DungeonUI");
                LoadSceneAdditive("CompartidoUI");
                break;
        }
    }

    // Para agregar escenas aditivas a la escena actual
    private AsyncOperation LoadSceneAdditive(string sceneName)
    {
        return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }
}
