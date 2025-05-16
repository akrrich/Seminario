using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    // Con un update manager se soluciona que se pueda seguir interactuando mientras esta en pantalla de carga

    // Agregar metodo futuro para cerrar el juego con pantalla de carga y guardar los datos en ese tiempo

    private static ScenesManager instance;

    [SerializeField] private GameObject loadingPanel;

    [SerializeField] private float duringTimeLoadingPanel;

#pragma warning disable 0414
    private bool isInLoadingScreen = false; // Esto sirve para el UpdateManager
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

                // Esperar 3 segundos aparte del tiempo de carga de escena, para que se carguen las aditivas
                yield return new WaitForSeconds(3);

                StartCoroutine(DisableLoadingPanelAfterSeconds());

                isInLoadingScreen = false;
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }


    private IEnumerator DisableLoadingPanelAfterSeconds()
    {
        // Esperar un frame
        yield return null;

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
                LoadSceneAdditive("MainMenuUI");
                break;

            case "Tabern":
                 LoadSceneAdditive("TabernUI");
                 LoadSceneAdditive("CompartidoUI");
                break;

            case "Dungeon":
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
