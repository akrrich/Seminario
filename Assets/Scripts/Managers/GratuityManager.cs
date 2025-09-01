using System.Collections.Generic;
using UnityEngine;

public class GratuityManager : Singleton<GratuityManager>
{
    [SerializeField] private GratuityManagerData gratuityManagerData;

    [SerializeField] private List<ClientFoodPaymentData> clientFoodPaymentsData;

    public GratuityManagerData GratuityManagerData { get => gratuityManagerData; }


    void Awake()
    {
        CreateSingleton(false);
    }


    public int GetPayment(ClientType client, FoodType foodType)
    {
        foreach (var entry in clientFoodPaymentsData)
        {
            if (entry.ClientType == client && entry.FoodType == foodType)
            {
                return entry.PaymentAmount;
            }
        }

        return 0;
    }

    public void TryGiveGratuity(int paymentAmount)
    {
        int roll = Random.Range(0, 100);
        if (roll >= gratuityManagerData.ProbabilityToGiveGratuity) return; 

        int randomGratuityRoll = Random.Range(0, 100);
        int cumulative = 0;

        foreach (var option in gratuityManagerData.GratuityOptionsPercentage)
        {
            cumulative += option.Probability;
            if (randomGratuityRoll < cumulative)
            {
                int gratuity = Mathf.RoundToInt(paymentAmount * option.GratuityPercentage / 100f);
                MoneyManager.Instance.AddMoney(gratuity);
                break;
            }
        }
    }
}
