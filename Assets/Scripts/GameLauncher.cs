using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameLauncher : MonoBehaviour
{
     /* Este script realiza lo siguiente: Debe estar en todas las escenas para poder hacer el testing
     correctamente, el script esta asignado a un prefab llamado GameLauncher, con el objetivo de que
     se inicie la escena data una vez, sin importar desde que escena se este runeando el juego, para 
     que los datos puedan persistir en memoria durante toda su ejecucion*/

    // Aclaracion: No se debe asignar el prefab con este script en las escenas de UI y la escena Data

    private static int counter = 0; // Statico para que viva en toda la ejecucion


    void Start()
    {
        InitializeGameLauncher();
    }


    private void InitializeGameLauncher()
    {
        if (counter > 0)
        {
            Destroy(gameObject);
            return;
        }

        else
        {
            StartCoroutine(LoadDataSceneAndDestroySelf());
        }
    }

    private IEnumerator LoadDataSceneAndDestroySelf()
    {
        AsyncOperation loadOpDataScene = SceneManager.LoadSceneAsync("Data", LoadSceneMode.Additive);

        yield return new WaitUntil(() => loadOpDataScene.isDone);

        SceneManager.UnloadSceneAsync("Data");

        counter++;
        Destroy(gameObject);
    }
}
