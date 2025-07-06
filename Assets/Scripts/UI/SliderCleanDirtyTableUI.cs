using UnityEngine;
using System;
using UnityEngine.UI;

public class SliderCleanDirtyTableUI : MonoBehaviour
{
    [SerializeField] private SliderCleanDiirtyTableUIData sliderCleanDiirtyTableUIData;

    [SerializeField] private Slider sliderCleanDirtyTable;

    private Table currentTable;

    private Action onActiveSlider, onDeactivateSlider;


    void Awake()
    {
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
        currentTable.CurrentCleanProgress = Mathf.Min(currentTable.CurrentCleanProgress, sliderCleanDiirtyTableUIData.MaxHoldTime);

        UpdateSliderValueFromCurrentTable(currentTable);

        if (currentTable.CurrentCleanProgress >= sliderCleanDiirtyTableUIData.MaxHoldTime)
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
        sliderCleanDirtyTable.value = table.CurrentCleanProgress / sliderCleanDiirtyTableUIData.MaxHoldTime;
    }

    private void DecreaseAllSliderValuesExceptCurrentTable()
    {
        for (int i = 0; i < TablesManager.Instance.Tables.Count; i++)
        {
            Table table = TablesManager.Instance.Tables[i];

            if (sliderCleanDirtyTable.gameObject.activeSelf && currentTable != null && table == currentTable)
            {
                continue;
            }

            table.CurrentCleanProgress -= Time.deltaTime;
            table.CurrentCleanProgress = Mathf.Max(table.CurrentCleanProgress, 0f);
        }
    }
}
