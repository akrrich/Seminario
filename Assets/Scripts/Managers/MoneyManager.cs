using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    private static MoneyManager instance;

    private TextMeshProUGUI moneyText;

    [SerializeField] private float initializeCurrentMoneyValue;
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
        currentMoney = initializeCurrentMoneyValue;
    }

    private void UpdateMoneyText()
    {
        moneyText.text = currentMoney.ToString();
    }
}
