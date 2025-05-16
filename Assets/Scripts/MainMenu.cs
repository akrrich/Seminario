using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private AudioSource buttonClick;


    void Awake()
    {
        GetComponents();
    }


    // Funcion asignada a boton en la UI
    public void ButtonPlay()
    {
        StartCoroutine(LoadSceneAfterButtonClick());
    }

    // Funcion asignada a boton en la UI
    public void ButtonExit()
    {
        StartCoroutine(CloseGameAfterClickButton());
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
