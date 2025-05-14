using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    private static ScenesManager instance;

    public static ScenesManager Instance { get => instance; }


    void Awake()
    {
        CreateSingleton();
        SetInitializedScene();
    }


    // Para pasar de una escena a otra
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    // Para agregar escenas aditivas a la escena actual
    public void LoadScene(string sceneName, LoadSceneMode mode)
    {
        SceneManager.LoadSceneAsync(sceneName, mode);
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

    // Este metodo solamente funciona para cuando se inicia el programa, es decir solamente una vez en toda la ejecucion
    private void SetInitializedScene()
    {
        Scene initializedCurrentScene = SceneManager.GetActiveScene();

        switch (initializedCurrentScene.name)
        {
            case "MainMenu":
                LoadScene("MainMenuUI", LoadSceneMode.Additive);
                break;

            case "Tabern":
                 LoadScene("TabernUI", LoadSceneMode.Additive);
                 LoadScene("CompartidoUI", LoadSceneMode.Additive);
                break;

            case "Dungeon":
                LoadScene("DungeonUI", LoadSceneMode.Additive);
                LoadScene("CompartidoUI", LoadSceneMode.Additive);
                break;
        }
    }
}
