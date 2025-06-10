using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class EventSystemMainMenu : MonoBehaviour
{
    private EventSystem eventSystem;

    private List<GameObject> buttonsMainMenu = new List<GameObject>();


    void Awake()
    {
        SuscribeToMainMenuEvent();
        GetComponents();
        StartCoroutine(InitializeFirstSelectedButtonByDefault());
    }


    void OnDestroy()
    {
        UnsuscribeToMainMenuEvent();
    }


    private void SuscribeToMainMenuEvent()
    {
        MainMenu.OnSendButtonsToEventSystem += GetButtonsMainMenuFromEvent;
    }

    private void UnsuscribeToMainMenuEvent()
    {
        MainMenu.OnSendButtonsToEventSystem -= GetButtonsMainMenuFromEvent;
    }

    private void GetComponents()
    {
        eventSystem = GetComponent<EventSystem>();
    }

    private void GetButtonsMainMenuFromEvent(List<GameObject> buttonsMainMenu)
    {
        this.buttonsMainMenu = buttonsMainMenu;
    }

    private IEnumerator InitializeFirstSelectedButtonByDefault()
    {
        // Esperar 2 segundos para que cargue la escena additiva de MainMenu
        yield return new WaitForSeconds(2);

        eventSystem.SetSelectedGameObject(buttonsMainMenu[0]);
    }
}
