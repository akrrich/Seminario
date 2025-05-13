using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    private static ScenesManager instance;

    private Scene initializedCurrentScene;

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

    private void SetInitializedScene()
    {
        initializedCurrentScene = SceneManager.GetActiveScene();

        switch (initializedCurrentScene.name)
        {
            case "Game":
                 LoadScene("TabernUI", LoadSceneMode.Additive);
                break;

            case "MainMenu":
                LoadScene("MainMenuUI", LoadSceneMode.Additive);
                break;

            case "Dungeon":
                LoadScene("DungeonUI", LoadSceneMode.Additive);
                break;
        }
    }
}
