using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BookManagerUI : Singleton<BookManagerUI>
{
    private List<IBookableUI> panelsBookTabern = new List<IBookableUI>();
    private List<IBookableUI> panelsBookDungeon = new List<IBookableUI>();

    private static event Action onHideOutlinesFromInteractableElements;

    private Scene currentScene;

    private int indexCurrentPanelOpen = 0;

    private bool isBookOpen = false;
    private bool isRightStickUsing = false;

    public static Action OnHideOutlinesFromInteractableElements { get => onHideOutlinesFromInteractableElements; set => onHideOutlinesFromInteractableElements = value; }

    public bool IsBookOpen { get => isBookOpen; }


    void Awake()
    {
        CreateSingleton(false);
        SuscribeToUpdateManagerEvent();
        SuscribeToPlayerControllerEvent();
        GetCurrentSceneName();
    }

    void Start()
    {
        FindPanelsInHierarchy();
    }

    // Simulacion de Update
    void UpdateBookManagerUI()
    {
        ChangePanelIndex();
    }

    void OnDestroy()
    {
        UnsuscribeToUpdateManagerEvent();
        UnsuscribeToPlayerControllerEvent();
    }


    private void SuscribeToUpdateManagerEvent()
    {
        UpdateManager.OnUpdate += UpdateBookManagerUI;
    }

    private void UnsuscribeToUpdateManagerEvent()
    {
        UpdateManager.OnUpdate -= UpdateBookManagerUI;
    }

    private void SuscribeToPlayerControllerEvent()
    {
        PlayerController.OnOpenOrCloseBook += OpenOrCloseBook;
    }

    private void UnsuscribeToPlayerControllerEvent()
    {
        PlayerController.OnOpenOrCloseBook -= OpenOrCloseBook;
    }

    private void GetCurrentSceneName()
    {
        currentScene = SceneManager.GetActiveScene();
    }

    private void FindPanelsInHierarchy()
    {
        if (ScenesManager.Instance.CurrentSceneName == "Tabern")
        {
            panelsBookTabern = Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<IBookableUI>().Where(p => ((MonoBehaviour)p).gameObject.scene.isLoaded).OrderBy(p => p.IndexPanel).ToList();
            return;
        }

        else if (ScenesManager.Instance.CurrentSceneName == "Dungeon")
        {
            panelsBookDungeon = Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<IBookableUI>().Where(p => ((MonoBehaviour)p).gameObject.scene.isLoaded).OrderBy(p => p.IndexPanel).ToList();
            return;
        }
    }

    private void OpenOrCloseBook()
    {
        if (PauseManager.Instance.IsGamePaused) return;

        onHideOutlinesFromInteractableElements?.Invoke();

        if (!isBookOpen)
        {
            DeviceManager.Instance.IsUIModeActive = true;
            isBookOpen = true;
            EnabledNextPanel(indexCurrentPanelOpen);
            return;
        }

        else if (isBookOpen)
        {
            DeviceManager.Instance.IsUIModeActive = false;
            isBookOpen = false;
            DisableCurrentPanel(indexCurrentPanelOpen);
            indexCurrentPanelOpen = 0;
            return;
        }
    }

    private void ChangePanelIndex()
    {
        if (!isBookOpen) return;
        if (PauseManager.Instance.IsGamePaused) return;

        float rightStickX = Input.GetAxis("RightStickHorizontal");

        if ((PlayerInputs.Instance.E() || rightStickX > 0.5f) && !isRightStickUsing)
        {
            isRightStickUsing = true;

            DisableCurrentPanel(indexCurrentPanelOpen);
            indexCurrentPanelOpen++;

            if (indexCurrentPanelOpen >= panelsBookTabern.Count)
            {
                indexCurrentPanelOpen = 0;
            }

            EnabledNextPanel(indexCurrentPanelOpen);
        }

        else if ((PlayerInputs.Instance.Q() || rightStickX < -0.5f) && !isRightStickUsing)
        {
            isRightStickUsing = true;

            DisableCurrentPanel(indexCurrentPanelOpen);
            indexCurrentPanelOpen--;

            if (indexCurrentPanelOpen < 0)
            {
                indexCurrentPanelOpen = panelsBookTabern.Count - 1;
            }

            EnabledNextPanel(indexCurrentPanelOpen);
        }

        if (Mathf.Abs(rightStickX) < 0.2f)
        {
            isRightStickUsing = false;
        }
    }

    private void DisableCurrentPanel(int currentIndex)
    {
        if (ScenesManager.Instance.CurrentSceneName == "Tabern")
        {
            panelsBookTabern[currentIndex].ClosePanel();
            return;
        }

        else if (ScenesManager.Instance.CurrentSceneName == "Dungeon")
        {
            panelsBookDungeon[currentIndex].ClosePanel();
            return;
        }
    }

    private void EnabledNextPanel(int currentIndex)
    {
        if (ScenesManager.Instance.CurrentSceneName == "Tabern")
        {
            panelsBookTabern[currentIndex].OpenPanel();
            return;
        }

        else if (ScenesManager.Instance.CurrentSceneName == "Dungeon")
        {
            panelsBookDungeon[currentIndex].OpenPanel();
            return;
        }
    }
}
