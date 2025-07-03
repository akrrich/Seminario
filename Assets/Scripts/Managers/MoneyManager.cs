using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    private static MoneyManager instance;

    [SerializeField] private MoneyManagerData moneyManagerData;

    private TextMeshProUGUI moneyText;

    private float currentMoney;

    public static MoneyManager Instance { get => instance; }

    public float CurrentMoney { get => currentMoney; }


    void Awake()
    {
        CreateSingleton();
        InitializeCurrentMoney();
        SuscribeToMoneyTextEvent();
    }


    public void AddMoney(float money)
    {
        currentMoney += money;
        UpdateMoneyText();
    }

    public void SubMoney(float money)
    {
        currentMoney -= money;
        UpdateMoneyText();
    }


    private void CreateSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
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
}
