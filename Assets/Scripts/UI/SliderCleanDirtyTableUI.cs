using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class SliderCleanDirtyTableUI : MonoBehaviour
{
    [SerializeField] private Slider sliderCleanDirtyTable;

    private List<Table> allTables =  new List<Table>();  

    private Table currentTable;

    private Action onActiveSlider, onDeactivateSlider;

    [SerializeField] private float maxHoldTime;


    void Awake()
    {
        FindAllTables();
        SuscribeToLamdaEvents();
        SuscribeToPlayerViewEvents();
        SuscribeToPlayerControllerEvents();
    }

    void Update()
    {
        DecreaseAllSliderValuesExceptCurrentTable();
    }

    void OnDestroy()
    {
        UnsuscribeToLamdaEvents();
        UnuscribeToPlayerViewEvents();
        UnsuscribeToPlayerControllerEvents();
    }

    /// <summary>
    /// Este metodo va a ser util para cuando se expanda la taberna, asi se agregan las nuevas mesas a la lista si es necesario
    /// </summary>
    public void AddNewTableToList()
    {

    }


    private void FindAllTables()
    {
        GameObject[] tableObjects = GameObject.FindGameObjectsWithTag("Table");

        foreach (GameObject obj in tableObjects)
        {
            Table table = obj.GetComponentInParent<Table>();
            if (table != null)
            {
                allTables.Add(table);
            }
        }
    }

    private void SuscribeToLamdaEvents()
    {
        onActiveSlider += () => ActivateOrDeactivateSlider(true);
        onDeactivateSlider += () => ActivateOrDeactivateSlider(false);
    }

    private void UnsuscribeToLamdaEvents()
    {
        onActiveSlider -= () => ActivateOrDeactivateSlider(true);
        onDeactivateSlider -= () => ActivateOrDeactivateSlider(false);
    }

    private void SuscribeToPlayerViewEvents()
    {
        PlayerView.OnActivateSliderCleanDirtyTable += onActiveSlider;
        PlayerView.OnDeactivateSliderCleanDirtyTable += onDeactivateSlider;
    }

    private void UnuscribeToPlayerViewEvents()
    {
        PlayerView.OnActivateSliderCleanDirtyTable -= onActiveSlider;
        PlayerView.OnDeactivateSliderCleanDirtyTable -= onDeactivateSlider;
    }

    private void SuscribeToPlayerControllerEvents()
    {
        PlayerController.OnCleanDirtyTableIncreaseSlider += IncreaseFromCurrentTable;
        PlayerController.OnCleanDirtyTableDecreaseSlider += DecreaseSliderFromCurrentTable;
    }

    private void UnsuscribeToPlayerControllerEvents()
    {
        PlayerController.OnCleanDirtyTableIncreaseSlider -= IncreaseFromCurrentTable;
        PlayerController.OnCleanDirtyTableDecreaseSlider -= DecreaseSliderFromCurrentTable;
    }

    private void ActivateOrDeactivateSlider(bool current)
    {
        if (!current)
        {
            currentTable = null;
        }

        sliderCleanDirtyTable.gameObject.SetActive(current);
    }

    private void IncreaseFromCurrentTable(Table table)
    {
        if (currentTable == null)
        {
            currentTable = table;
        }

        currentTable.CurrentCleanProgress += Time.deltaTime;
        currentTable.CurrentCleanProgress = Mathf.Min(currentTable.CurrentCleanProgress, maxHoldTime);

        UpdateSliderValueFromCurrentTable(currentTable);

        if (currentTable.CurrentCleanProgress >= maxHoldTime)
        {
            currentTable.CurrentCleanProgress = 0;
            sliderCleanDirtyTable.value = sliderCleanDirtyTable.minValue;
            onDeactivateSlider?.Invoke();
            table.SetDirty(false);
        }
    }

    private void DecreaseSliderFromCurrentTable(Table table)
    {
        if (currentTable != table)
        {
            currentTable = table;
        }

        if (currentTable != null)
        {
            currentTable.CurrentCleanProgress -= Time.deltaTime;
            currentTable.CurrentCleanProgress = Mathf.Max(currentTable.CurrentCleanProgress, 0f);

            UpdateSliderValueFromCurrentTable(currentTable);
        }
    }

    private void UpdateSliderValueFromCurrentTable(Table table)
    {
        sliderCleanDirtyTable.value = table.CurrentCleanProgress / maxHoldTime;
    }

    private void DecreaseAllSliderValuesExceptCurrentTable()
    {
        for (int i = 0; i < allTables.Count; i++)
        {
            Table table = allTables[i];

            if (sliderCleanDirtyTable.gameObject.activeSelf && currentTable != null && table == currentTable)
            {
                continue;
            }

            table.CurrentCleanProgress -= Time.deltaTime;
            table.CurrentCleanProgress = Mathf.Max(table.CurrentCleanProgress, 0f);
        }
    }
}
