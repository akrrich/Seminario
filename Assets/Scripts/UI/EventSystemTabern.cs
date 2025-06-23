using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class EventSystemTabern : MonoBehaviour
{
    private EventSystem eventSystem;

    //private List<GameObject> buttonsPause = new List<GameObject>();
    //private List<GameObject> buttonsCooking = new List<GameObject>();


    void Awake()
    {
        SuscribeToUIEvents();
        GetComponents();
    }

    void OnDestroy()
    {
        UnsuscribeToUIEvents();
    }


    private void SuscribeToUIEvents()
    {
        //PauseManager.OnSendButtonsToEventSystem += GetButtonsPauseFromEvent;
        PauseManager.OnSetSelectedCurrentGameObject += SetSelectedCurrentGameObject;
        PauseManager.OnClearSelectedCurrentGameObject += ClearCurrentSelectedGameObject;

        //CookingManagerUI.OnSendButtonsToEventSystem += GetButtonsCookingFromEvent;
        CookingManagerUI.OnSetSelectedCurrentGameObject += SetSelectedCurrentGameObject;
        CookingManagerUI.OnClearSelectedCurrentGameObject += ClearCurrentSelectedGameObject;

        AdministratingManagerUI.OnSetSelectedCurrentGameObject += SetSelectedCurrentGameObject;
        AdministratingManagerUI.OnClearSelectedCurrentGameObject += ClearCurrentSelectedGameObject;
    }

    private void UnsuscribeToUIEvents()
    {
        //PauseManager.OnSendButtonsToEventSystem -= GetButtonsPauseFromEvent;
        PauseManager.OnSetSelectedCurrentGameObject -= SetSelectedCurrentGameObject;
        PauseManager.OnClearSelectedCurrentGameObject -= ClearCurrentSelectedGameObject;

        //CookingManagerUI.OnSendButtonsToEventSystem -= GetButtonsCookingFromEvent;
        CookingManagerUI.OnSetSelectedCurrentGameObject -= SetSelectedCurrentGameObject;
        CookingManagerUI.OnClearSelectedCurrentGameObject -= ClearCurrentSelectedGameObject;

        AdministratingManagerUI.OnSetSelectedCurrentGameObject -= SetSelectedCurrentGameObject;
        AdministratingManagerUI.OnClearSelectedCurrentGameObject -= ClearCurrentSelectedGameObject;
    }

    private void GetButtonsPauseFromEvent(List<GameObject> buttonsPause)
    {
        //this.buttonsPause = buttonsPause;
    }

    private void GetButtonsCookingFromEvent(List<GameObject> buttonsCooking)
    {
        //this.buttonsCooking = buttonsCooking;
    }

    private void GetComponents()
    {
        eventSystem = GetComponent<EventSystem>();
    }

    // Sirve para selecionar un GameObject
    private void SetSelectedCurrentGameObject(GameObject currentGameObject)
    {
        eventSystem.SetSelectedGameObject(currentGameObject);
    }

    // Sirve para limpiar el GameObject seleccionado, util para cuando salimos de modo UI
    private void ClearCurrentSelectedGameObject()
    {
        eventSystem.SetSelectedGameObject(null);
    }
}
