using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class AdministratingManagerUI : MonoBehaviour
{
    /// <summary>
    /// Preguntar por el grupo el tema de ajustar que se deseleccione el boton cuando sale el mouse
    /// </summary>
    /// 
    [SerializeField] private GameObject panelAdministrating;

    [SerializeField] private GameObject panelIngredients;
    [SerializeField] private GameObject panelTabern; 

    /// <summary>
    /// Agregar que sonido de cancelacion si no puede comprar el ingrediente
    /// </summary>
    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private AudioSource buttonSelected;

    private List<ZoneUnlock> zoneUnlocks = new List<ZoneUnlock>();
    private Image currentImageZoneUnlock;
    private TextMeshProUGUI textPriceCurrentZoneUnlock;

    private List<Button> buttonsPanelAdministrating = new List<Button>(); // Botones de arriba de todo

    private List<GameObject> buttonsIngredients = new List<GameObject>();
    private List<GameObject> buttonsTabern = new List<GameObject>();

    private GameObject lastSelectedButtonFromAdminPanel;

    private event Action onEnterAdmin, onExitAdmin;

    private static event Action<GameObject> onSetSelectedCurrentGameObject;
    private static event Action onClearSelectedCurrentGameObject;

    /// <summary>
    ///  Verificar si el error de que no reproduce el sonido despues de despausar el juego sigue ocurriendo cuando se agregan elementos en el PanelTabern
    /// </summary>
    private bool ignoreFirstButtonSelected = true;

    public static Action<GameObject> OnSetSelectedCurrentGameObject { get => onSetSelectedCurrentGameObject; set => onSetSelectedCurrentGameObject = value; }
    public static Action OnClearSelectedCurrentGameObject { get => onClearSelectedCurrentGameObject; set => onClearSelectedCurrentGameObject = value; }


    void Awake()
    {
        InitializeLambdaEvents();
        SuscribeToPlayerViewEvents();
        SuscribeToPauseManagerRestoreSelectedGameObjectEvent();
        GetComponents();
    }

    void Update()
    {
        CheckLastSelectedButtonIfAdminPanelIsOpen();
        CheckJoystickInputsToInteractWithPanels();
    }

    void OnDestroy()
    {
        UnsuscribeToPlayerViewEvents();
        UnscribeToPauseManagerRestoreSelectedGameObjectEvent();
    }


    // Funcion asignada a botones en la UI para setear el selected GameObject del EventSystem con Mouse
    public void SetButtonAsSelectedGameObjectIfHasBeenHover(int indexButton)
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(buttonsIngredients[indexButton]);
        }
    }

    // Funcion asignada a botones en la UI para reproducir el sonido selected
    public void PlayAudioButtonSelectedWhenChangeSelectedGameObjectExceptFirstTime()
    {
        if (!ignoreFirstButtonSelected)
        {
            buttonSelected.Play();
            return;
        }

        ignoreFirstButtonSelected = false;
    }

    /// <summary>
    /// Funcion hay que ajustarla en un futuro para que funcione con varias zonas de desbloqueo, actualmente funciona desde codigo
    /// </summary>
    public void ShowCurrentZoneInformation(int index)
    {
        currentImageZoneUnlock.sprite = zoneUnlocks[index].ImageZoneUnlock;
        textPriceCurrentZoneUnlock.text = "Price: " + zoneUnlocks[index].Cost.ToString();
    }

    // Funcion asignada a boton UI
    public void ButtonBuyIngredient(string ingredientName)
    {
        if (Enum.TryParse(ingredientName, out IngredientType ingredient))
        {
            int price = IngredientInventoryManager.Instance.GetPriceOfIngredient(ingredient);

            if (MoneyManager.Instance.CurrentMoney >= price)
            {
                buttonClick.Play();
                IngredientInventoryManager.Instance.IncreaseIngredientStock(ingredient);
                MoneyManager.Instance.SubMoney(price);
            }
        }
    }

    // Funcion asignada a boton UI
    public void ButtonUnlockNewZone(int index)
    {
        if (zoneUnlocks[index].IsUnlocked) return;

        int price = zoneUnlocks[index].Cost;

        if (MoneyManager.Instance.CurrentMoney >= price)
        {
            buttonClick.Play();
            zoneUnlocks[index].UnlockZone();
            MoneyManager.Instance.SubMoney(price);
        }
    }

    // Funciones asignadas a OnPointerEnter para el mouse y combinadas con los Inputs del joystick
    // El objetivo es que se abran los paneles correctamente y se cierren correctamente
    public void SetPanelIngredients()
    {
        // Color blanco
        ColorBlock color = buttonsPanelAdministrating[0].colors;
        color.normalColor = new Color32(0xFF, 0xFF, 0xFF, 0xFF);

        if (buttonsPanelAdministrating[0].colors == color)
        {
            // Aca se podria agregar que selecione el ultimo que tenia antes
            onSetSelectedCurrentGameObject?.Invoke(buttonsIngredients[0]);

            buttonSelected.Play();

            SetButtonNormalColorInWhite(buttonsPanelAdministrating[1]);
            panelTabern.SetActive(false);

            SetButtonNormalColorInGreen(buttonsPanelAdministrating[0]);
            panelIngredients.SetActive(true);
        }
    }

    public void SetPanelTabern()
    {
        // Color blanco
        ColorBlock color = buttonsPanelAdministrating[1].colors;
        color.normalColor = new Color32(0xFF, 0xFF, 0xFF, 0xFF);

        if (buttonsPanelAdministrating[1].colors == color)
        {
            // Aca se podria agregar que selecione el ultimo que tenia antes
            onSetSelectedCurrentGameObject?.Invoke(buttonsTabern[0]);
            ShowCurrentZoneInformation(0);

            buttonSelected.Play();            

            SetButtonNormalColorInWhite(buttonsPanelAdministrating[0]);
            panelIngredients.SetActive(false);

            SetButtonNormalColorInGreen(buttonsPanelAdministrating[1]);
            panelTabern.SetActive(true);
        }
    }


    private void InitializeLambdaEvents()
    {
        onEnterAdmin += () => ActiveOrDeactivatePanel(true);
        onExitAdmin += () => ActiveOrDeactivatePanel(false);
    }

    private void SuscribeToPlayerViewEvents()
    {
        PlayerView.OnEnterInAdministrationMode += onEnterAdmin;
        PlayerView.OnExitInAdministrationMode += onExitAdmin;
    }

    private void UnsuscribeToPlayerViewEvents()
    {
        PlayerView.OnEnterInAdministrationMode -= onEnterAdmin;
        PlayerView.OnExitInAdministrationMode -= onExitAdmin;
    }

    private void SuscribeToPauseManagerRestoreSelectedGameObjectEvent()
    {
        PauseManager.OnRestoreSelectedGameObject += RestoreLastSelectedGameObjectIfGameWasPausedDuringAdministratingUI;
    }

    private void UnscribeToPauseManagerRestoreSelectedGameObjectEvent()
    {
        PauseManager.OnRestoreSelectedGameObject -= RestoreLastSelectedGameObjectIfGameWasPausedDuringAdministratingUI;
    }

    private void GetComponents()
    {
        foreach (Transform childs in panelAdministrating.transform)
        {
            buttonsPanelAdministrating.Add(childs.GetComponent<Button>());
        }

        FindGameObjectsReferences(panelIngredients, buttonsIngredients);
        FindGameObjectsReferences(panelTabern, buttonsTabern);

        GameObject ZonesToUnlockFather = GameObject.Find("ZonesToUnlock");
        foreach (Transform childs in ZonesToUnlockFather.transform)
        {
            zoneUnlocks.Add(childs.GetComponent<ZoneUnlock>());
        }

        currentImageZoneUnlock = panelTabern.transform.transform.Find("ImageBorderCurrentZone").transform.Find("ImageCurrentZone").GetComponent<Image>();
        textPriceCurrentZoneUnlock = panelTabern.transform.transform.Find("ImageBorderCurrentZone").transform.Find("TextPriceCurrentZone").GetComponent<TextMeshProUGUI>();
    }

    private void FindGameObjectsReferences(GameObject panel, List<GameObject> buttons)
    {
        foreach (Transform childs in panel.transform)
        {
            buttons.Add(childs.gameObject);
        }
    }

    private void ActiveOrDeactivatePanel(bool state)
    {
        panelAdministrating.SetActive(state);

        if (state)
        {
            SetButtonNormalColorInGreen(buttonsPanelAdministrating[0]);

            panelIngredients.SetActive(state);

            DeviceManager.Instance.IsUIModeActive = true;
            onSetSelectedCurrentGameObject?.Invoke(buttonsIngredients[0]);
        }

        else
        {
            SetButtonNormalColorInWhite(buttonsPanelAdministrating[0]);
            SetButtonNormalColorInWhite(buttonsPanelAdministrating[1]);

            panelIngredients.SetActive(state);
            panelTabern.SetActive(state);

            ignoreFirstButtonSelected = true;
            DeviceManager.Instance.IsUIModeActive = false;
            onClearSelectedCurrentGameObject?.Invoke();
        }
    }

    private void RestoreLastSelectedGameObjectIfGameWasPausedDuringAdministratingUI()
    {
        if (panelAdministrating.activeSelf)
        {
            ignoreFirstButtonSelected = true;
            DeviceManager.Instance.IsUIModeActive = true;
            EventSystem.current.SetSelectedGameObject(lastSelectedButtonFromAdminPanel);
        }
    }

    private void CheckLastSelectedButtonIfAdminPanelIsOpen()
    {
        if (EventSystem.current != null && PauseManager.Instance != null && !PauseManager.Instance.IsGamePaused && panelAdministrating.activeSelf)
        {
            lastSelectedButtonFromAdminPanel = EventSystem.current.currentSelectedGameObject;
        }
    }

    private void SetButtonNormalColorInWhite(Button currentButton)
    {
        ColorBlock color = currentButton.colors;
        color.normalColor = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
        currentButton.colors = color;
    }

    private void SetButtonNormalColorInGreen(Button currentButton)
    {
        ColorBlock color = currentButton.colors;
        color.normalColor = new Color32(0x28, 0xFF, 0x00, 0xFF);
        currentButton.colors = color;
    }

    private void CheckJoystickInputsToInteractWithPanels()
    {
        if (PlayerInputs.Instance.R1())
        {
            SetPanelTabern();
        }

        if (PlayerInputs.Instance.L1())
        {
            SetPanelIngredients();
        }
    }
}
