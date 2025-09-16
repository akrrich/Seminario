using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CookingManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject rootGameObject; // GameObject padre con los botones de hijos
    [SerializeField] private GameObject panelInformation;
    [SerializeField] private GameObject cookingCamera;

    /// <summary>
    /// Agregar ruido de cancelacion si no tiene ingredientes para cocinar una receta
    /// </summary>
    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private AudioSource buttonSelected;

    [SerializeField] private List<RecipeInformationUI> recipesInformationUI;

    private List<GameObject> buttonsInformationReciepes  = new List<GameObject>();

    private GameObject lastSelectedButtonFromCookingPanel;

    private static event Action<string> onButtonGetFood;

    private event Action onEnterCook, onExitCook;

    private static event Action<GameObject> onSetSelectedCurrentGameObject;
    private static event Action onClearSelectedCurrentGameObject;

    private List<IngredientType> selectedIngredients = new List<IngredientType>();

    private bool ignoreFirstButtonSelected = true;

    public static Action<string> OnButtonSetFood { get => onButtonGetFood; set => onButtonGetFood = value; }

    public static Action<GameObject> OnSetSelectedCurrentGameObject { get => onSetSelectedCurrentGameObject; set => onSetSelectedCurrentGameObject = value; }
    public static Action OnClearSelectedCurrentGameObject { get => onClearSelectedCurrentGameObject; set => onClearSelectedCurrentGameObject = value; }


    void Awake()
    {
        SuscribeToUpdateManagerEvent();
        InitializeLambdaEvents();
        SuscribeToPlayerViewEvents();
        SuscribeToPauseManagerRestoreSelectedGameObjectEvent();
        GetComponents();
    }

    // Simulacion de Update
    void UpdateCookingManagerUI()
    {
        CheckLastSelectedButtonIfCookingPanelIsOpen();
    }

    void OnDestroy()
    {
        UnscribeToUpdateManagerEvent();
        UnSuscribeToPlayerViewEvents();
        UnscribeToPauseManagerRestoreSelectedGameObjectEvent();
    }


    // Funcion asignada a botones en la UI para setear el selected GameObject del EventSystem con Mouse
    public void SetButtonAsSelectedGameObjectIfHasBeenHover(int indexButton)
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(buttonsInformationReciepes[indexButton]);
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

    // Funcion asignada a event trigger de la UI para mostrar la informacion de las recetas
    public void ShowInformationRecipe(string foodTypeName)
    {
        if (!Enum.TryParse(foodTypeName, out FoodType foodType)) return;

        var recipe = IngredientInventoryManager.Instance.GetRecipe(foodType);
        if (recipe == null) return;

        for (int i = 0; i < recipesInformationUI.Count; i++)
        {
            if (i < recipe.Ingridients.Count)
            {
                var ing = recipe.Ingridients[i];
                recipesInformationUI[i].IngredientAmountText.text = ing.Amount.ToString();

                if (IngredientInventoryManager.Instance.IngredientDataDict.TryGetValue(ing.IngredientType, out var data))
                {
                    recipesInformationUI[i].IngredientImage.sprite = data.Sprite;
                }
            }

            else
            {
                recipesInformationUI[i].IngredientAmountText.text = "";
                recipesInformationUI[i].IngredientImage.sprite = null;
            }
        }
    }

    public void ButtonSelectCurrentIngredient(string ingredientType)
    {
        if (!Enum.TryParse(ingredientType, out IngredientType ingredient))
        {
            Debug.LogError($"El ingrediente {ingredientType} no existe en IngredientType.");
            return;
        }

        if (selectedIngredients.Contains(ingredient))
            selectedIngredients.Remove(ingredient);
        else
            selectedIngredients.Add(ingredient);

        buttonSelected.Play();
    }

    public void CookSelectedIngredients()
    {
        foreach (var recipe in IngredientInventoryManager.Instance.GetAllRecipes())
        {
            // Ingredientes de la receta
            var recipeIngredients = recipe.Ingridients.Select(i => i.IngredientType).ToList();

            // Verificamos que los seleccionados coincidan exactamente con los de la receta
            if (selectedIngredients.Count == recipeIngredients.Count &&
                !selectedIngredients.Except(recipeIngredients).Any())
            {
                // Ahora verificamos stock
                bool canCraft = true;
                foreach (var ing in recipe.Ingridients)
                {
                    if (IngredientInventoryManager.Instance.GetStock(ing.IngredientType) < ing.Amount)
                    {
                        canCraft = false;
                        break;
                    }
                }

                if (canCraft)
                {
                    // Cocinar
                    buttonClick.Play();
                    onButtonGetFood?.Invoke(recipe.FoodType.ToString());

                    Debug.Log($"Cocinaste {recipe.FoodType}!");
                    selectedIngredients.Clear();
                    return;
                }
            }
        }

        Debug.Log("No hay receta con esos ingredientes o no alcanza el stock.");
    }

    private void SuscribeToUpdateManagerEvent()
    {
        UpdateManager.OnUpdate += UpdateCookingManagerUI;
    }

    private void UnscribeToUpdateManagerEvent()
    {
        UpdateManager.OnUpdate -= UpdateCookingManagerUI;
    }

    private void InitializeLambdaEvents()
    {
        onEnterCook += () => ActiveOrDeactivateRootGameObject(true);
        onExitCook += () => ActiveOrDeactivateRootGameObject(false);
    }

    private void SuscribeToPlayerViewEvents()
    {
        PlayerView.OnEnterInCookMode += onEnterCook;
        PlayerView.OnExitInCookMode += onExitCook;
    }

    private void UnSuscribeToPlayerViewEvents()
    {
        PlayerView.OnEnterInCookMode -= onEnterCook;
        PlayerView.OnExitInCookMode -= onExitCook;
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
        Transform rootButtonsInformationReciepes = rootGameObject.transform.Find("ButtonsInformationReciepes");

        foreach (Transform childs in rootButtonsInformationReciepes.transform)
        {
            buttonsInformationReciepes.Add(childs.gameObject);
        }
    }

    private void ActiveOrDeactivateRootGameObject(bool state)
    {
        rootGameObject.SetActive(state);
        panelInformation.SetActive(state);
        cookingCamera.SetActive(state);

        if (state)
        {
            DeviceManager.Instance.IsUIModeActive = true;
            onSetSelectedCurrentGameObject?.Invoke(buttonsInformationReciepes[0]);
        }

        else
        {
            ignoreFirstButtonSelected = true;
            DeviceManager.Instance.IsUIModeActive = false;
            onClearSelectedCurrentGameObject?.Invoke();
        }
    }

    private void RestoreLastSelectedGameObjectIfGameWasPausedDuringAdministratingUI()
    {
        if (rootGameObject.activeSelf)
        {
            ignoreFirstButtonSelected = true;
            DeviceManager.Instance.IsUIModeActive = true;
            EventSystem.current.SetSelectedGameObject(lastSelectedButtonFromCookingPanel);
        }
    }

    private void CheckLastSelectedButtonIfCookingPanelIsOpen()
    {
        if (EventSystem.current != null && PauseManager.Instance != null && !PauseManager.Instance.IsGamePaused && rootGameObject.activeSelf)
        {
            lastSelectedButtonFromCookingPanel = EventSystem.current.currentSelectedGameObject;
        }
    }
}

[Serializable]
public class RecipeInformationUI
{
    [SerializeField] private Image ingredientImage;
    [SerializeField] private TextMeshProUGUI ingredientAmountText;

    public Image IngredientImage { get => ingredientImage; }
    public TextMeshProUGUI IngredientAmountText { get => ingredientAmountText; }
}