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
        GetComponents();
        InitializeCurrentMoney();
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
        }
    }

    private void GetComponents()
    {
        moneyText = GameObject.Find("MoneyText").GetComponent<TextMeshProUGUI>();
    }

    private void InitializeCurrentMoney()
    {
        currentMoney = initializeCurrentMoneyValue;
        moneyText.text = currentMoney.ToString();
    }

    private void UpdateMoneyText()
    {
        moneyText.text = currentMoney.ToString();
    }
}
