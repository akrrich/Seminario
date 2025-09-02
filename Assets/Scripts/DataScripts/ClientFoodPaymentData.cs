using UnityEngine;

[CreateAssetMenu(fileName = "CliendFoodPaymentData", menuName = "ScriptableObjects/Tabern/Create New CliendFoodPaymentData")]
public class ClientFoodPaymentData : ScriptableObject
{
    [SerializeField] private ClientType clientType;
    [SerializeField] private FoodType foodType;
    [SerializeField] private int paymentAmount;

    public ClientType ClientType { get => clientType; }
    public FoodType FoodType { get => foodType; }
    public int PaymentAmount { get => paymentAmount; }
}
