using UnityEngine;

[CreateAssetMenu(fileName = "MoneyManagerData", menuName = "ScriptableObjects/Create New MoneyManagerData")]
public class MoneyManagerData : ScriptableObject
{
    [SerializeField] private float initializeCurrentMoneyValue;

    public float InitializeCurrentMoneyValue { get => initializeCurrentMoneyValue; }
}
