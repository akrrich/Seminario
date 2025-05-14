using System;
using UnityEngine;
using TMPro;

public class MoneyManagerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private static event Action<TextMeshProUGUI> onTextGetComponent;

    public static Action<TextMeshProUGUI> OnTextGetComponent { get => onTextGetComponent; set => onTextGetComponent = value; }


    void Awake()
    {
        InvokeEvent();
    }


    private void InvokeEvent()
    {
        onTextGetComponent?.Invoke(moneyText);
    }
}
