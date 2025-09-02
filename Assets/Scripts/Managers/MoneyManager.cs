using TMPro;
using UnityEngine;

public class MoneyManager : Singleton<MoneyManager>
{
    [SerializeField] private MoneyManagerData moneyManagerData;

    [SerializeField] private ObjectPooler floatingMoneyTextPool;
    [SerializeField] private FloatingMoneyText floatingMoneyText;

    private TextMeshProUGUI moneyText;

    private float currentMoney;

    public float CurrentMoney { get => currentMoney; }


    void Awake()
    {
        CreateSingleton(true);
        InitializeCurrentMoney();
        SuscribeToMoneyTextEvent();
    }


    public void AddMoney(float amount)
    {
        currentMoney += amount;
        UpdateMoneyText();
        ShowFloatingMoneyText(amount, true);
    }

    public void SubMoney(float amount)
    {
        currentMoney -= amount;
        if (currentMoney < 0)
        {
            currentMoney = 0;
        }

        UpdateMoneyText();
        ShowFloatingMoneyText(amount, false);
    }


    // No es necesario desuscribirse porque es singleton
    private void SuscribeToMoneyTextEvent()
    {
        MoneyManagerUI.OnTextGetComponent += GetComponentFromEvent;
    }

    private void GetComponentFromEvent(TextMeshProUGUI moneyText)
    {
        this.moneyText = moneyText;

        UpdateMoneyText();
    }

    private void InitializeCurrentMoney()
    {
        currentMoney = moneyManagerData.InitializeCurrentMoneyValue;
    }

    private void UpdateMoneyText()
    {
        moneyText.text = currentMoney.ToString();
    }

    private void ShowFloatingMoneyText(float amount, bool positive)
    {
        /*FloatingMoneyText obj = floatingMoneyTextPool.GetObjectFromPool<FloatingMoneyText>();

        if (positive)
        {
            obj.TextAmount.text = "+" + amount.ToString();
        }

        else
        {
            obj.TextAmount.text = "-" + amount.ToString();
        }

        StartCoroutine(floatingMoneyTextPool.ReturnObjectToPool(obj, obj.MaxTimeToReturnObjectToPool));*/

        FloatingMoneyText go = Instantiate(floatingMoneyText, moneyText.transform.position, Quaternion.identity);

        if (positive)
        {
            go.TextAmount.text = "+" + amount.ToString();
        }

        else
        {
            go.TextAmount.text = "-" + amount.ToString();
        }

        Destroy(go.gameObject, go.MaxTimeToReturnObjectToPool);
    }
}
