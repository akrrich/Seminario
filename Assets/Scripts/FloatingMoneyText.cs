using TMPro;
using UnityEngine;

public class FloatingMoneyText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textAmount;

    private float maxTimeToReturnObjectToPool = 0.45f;

    public TextMeshProUGUI TextAmount { get => textAmount; set => textAmount = value; }

    public float MaxTimeToReturnObjectToPool { get => maxTimeToReturnObjectToPool; }


    void Awake()
    {
        GetComponent();
    }


    private void GetComponent()
    {
        textAmount = GetComponentInChildren<TextMeshProUGUI>();
    }
}
