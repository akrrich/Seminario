using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientInventoryManagerUI : MonoBehaviour
{
    private PlayerModel playerModel;

    [SerializeField] private RawImage inventoryPanel;

    private Transform slotParentObject;
    private List<Transform> slotPositions = new List<Transform>();
    private Dictionary<IngredientType, (GameObject slot, TextMeshProUGUI text)> ingredientSlots = new();

    private bool isInventoryOpenUI = false;


    void Awake()
    {
        SuscribeToPlayerViewEvent();
        SuscribeToPlayerModelEvent();
        GetComponents();
        InitializeSlots();
    }

    void Update()
    {
        EnabledOrDisabledInventoryPanel();
    }

    void OnDestroy()
    {
        UnsuscribeToPlayerViewEvent();
        UnsuscribeToPlayerModelEvent();
    }


    private void SuscribeToPlayerViewEvent()
    {
        PlayerView.OnDeactivateInventoryFoodUI += HideInventory;
    }

    private void UnsuscribeToPlayerViewEvent()
    {
        PlayerView.OnDeactivateInventoryFoodUI -= HideInventory;
    }

    private void SuscribeToPlayerModelEvent()
    {
        PlayerModel.OnPlayerInitialized += GetPlayerModelReferenceFromEvent;
    }  

    private void UnsuscribeToPlayerModelEvent()
    {
        PlayerModel.OnPlayerInitialized -= GetPlayerModelReferenceFromEvent;
    }

    private void GetComponents()
    {
        slotParentObject = GameObject.Find("SlotObjects").transform;

        Transform slotParentPositions = GameObject.Find("SlotTransforms").transform;
        foreach (Transform slotChild in slotParentPositions)
        {
            slotPositions.Add(slotChild);
        }
    }

    private void InitializeSlots()
    {
        foreach (var slotPrefab in IngredientInventoryManager.Instance.IngredientsData)
        {
            IngredientType type = slotPrefab.IngredientType;
            if (!IngredientInventoryManager.Instance.GetAllIngredients().Contains(type))
                continue;

            int index = slotPositions.Count > ingredientSlots.Count ? ingredientSlots.Count : -1;
            if (index == -1) break;

            GameObject slotInstance = Instantiate(slotPrefab.PrefabSlotInventoryUI, slotPositions[index].position, Quaternion.identity, slotParentObject);
            slotInstance.SetActive(false);

            TextMeshProUGUI stockText = slotInstance.GetComponentInChildren<TextMeshProUGUI>();
            ingredientSlots[type] = (slotInstance, stockText);
        }
    }

    private void ShowInventory()
    {
        isInventoryOpenUI = true;
        inventoryPanel.enabled = true;

        foreach (var kvp in ingredientSlots)
        {
            kvp.Value.slot.SetActive(true);
            int stock = IngredientInventoryManager.Instance.GetStock(kvp.Key);
            kvp.Value.text.text = stock.ToString();
        }
    }

    private void HideInventory()
    {
        isInventoryOpenUI = false;
        inventoryPanel.enabled = false;

        foreach (var kvp in ingredientSlots)
        {
            kvp.Value.slot.SetActive(false);
        }
    }

    private void GetPlayerModelReferenceFromEvent(PlayerModel playerModel)
    {
        if (this.playerModel == null)
        {
            this.playerModel = playerModel;
        }
    }

    private void EnabledOrDisabledInventoryPanel()
    {
        if (playerModel != null && !playerModel.IsCooking && !playerModel.IsAdministrating && !PauseManager.Instance.IsGamePaused)
        {
            if (PlayerInputs.Instance.Inventory())
            {
                (isInventoryOpenUI ? (Action)HideInventory : ShowInventory)();
            }
        }
    }
}
