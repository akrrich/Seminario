using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientInventoryManagerUI : MonoBehaviour
{
    private PlayerModel playerModel;

    [SerializeField] private List<GameObject> startSlotPrefabs;
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
        var ingredientTypes = IngredientInventoryManager.Instance.GetAllIngredients();

        for (int i = 0; i < startSlotPrefabs.Count && i < slotPositions.Count; i++)
        { 
            var type = ingredientTypes[i];

            GameObject slotInstance = Instantiate(startSlotPrefabs[i], slotPositions[i].position, Quaternion.identity, slotParentObject);
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
        if (playerModel != null && !playerModel.IsCooking && !playerModel.IsAdministrating)
        {
            if (PlayerInputs.Instance.Inventory())
            {
                (isInventoryOpenUI ? (Action)HideInventory : ShowInventory)();
            }
        }
    }
}
