using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button[] buttonsClickOnce; // Para los botones que se pueden presionar una vez

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
        buttonsClickOnce[0].interactable = false;

        yield return new WaitForSeconds(buttonClick.clip.length);

        ScenesManager.Instance.LoadScene("Tabern");
        ScenesManager.Instance.LoadScene("TabernUI", LoadSceneMode.Additive);
    }

    private IEnumerator CloseGameAfterClickButton()
    {
        buttonClick.Play();
        buttonsClickOnce[1].interactable = false;

        yield return new WaitForSeconds(buttonClick.clip.length);

        Application.Quit();
    }
}
