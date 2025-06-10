using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    /// <summary>
    /// Con un update manager se soluciona que se pueda seguir interactuando mientras esta en pantalla de carga
    /// </summary>

    /// <sumary>
    /// Agregar metodo futuro para cerrar el juego con pantalla de carga y guardar los datos en ese tiempo
    /// </sumary>

    private static ScenesManager instance;

    [SerializeField] private GameObject loadingPanel;

    [SerializeField] private float duringTimeLoadingPanel;

#pragma warning disable 0414
    private bool isInLoadingScreen = false; /// <summary>
    /// Esto sirve para el UpdateManager, pero por ahora no srive de NADA
    /// </summary>
#pragma warning restore 0414 

    public static ScenesManager Instance { get => instance; }


    void Awake()
    {
        CreateSingleton();
        DontDestroyOnLoadLoadingPanelInCanvas();
        SetInitializedScene();
    }


    // Para pasar de una escena a otra con pantalla de carga
    public IEnumerator LoadScene(string sceneName, string[] additiveScenes)
    {
        loadingPanel.SetActive(true);
        isInLoadingScreen = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        float elapsedTime = 0f;

        while (!asyncLoad.isDone)
        {
            elapsedTime += Time.deltaTime;

            if (asyncLoad.progress >= 0.9f && elapsedTime >= duringTimeLoadingPanel)
            {
                if (additiveScenes != null)
                {
                    for (int i = 0; i < additiveScenes.Length; i++)
                    {
                        AsyncOperation additiveLoad = LoadSceneAdditive(additiveScenes[i]);
                    }
                }

                StartCoroutine(DisableLoadingPanelAfterSeconds());

                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }


    // Esto sirve para que una vez cargada la nueva escena, espere 3 segundos para desactivar el panel, para que permita cargar Awake y Start de la nueva escena cargada
    private IEnumerator DisableLoadingPanelAfterSeconds()
    {
        yield return new WaitForSeconds(3);

        isInLoadingScreen = false;
        loadingPanel.SetActive(false);
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
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void DontDestroyOnLoadLoadingPanelInCanvas()
    {
        if (loadingPanel != null)
        {
            DontDestroyOnLoad(loadingPanel.transform.root.gameObject);
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

            case "Tabern":
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
