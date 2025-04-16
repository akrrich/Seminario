using TMPro;
using UnityEngine;

public class ShowCookMessage : MonoBehaviour
{
    private TextMeshProUGUI cookMessageDesplayText;


    void Awake()
    {
        SuscribeToPlayerViewEvents();
        GetComponents();
    }

    void Start()
    {
        InitializeReferencs();
    }

    void OnDestroy()
    {
        UnSuscribeToPlayerViewEvents();
    }


    private void SuscribeToPlayerViewEvents()
    {
        PlayerView.OnEnterInCookModeMessage += ShowEnterMessageText;
        PlayerView.OnExitInCookModeMessage += DisapearMessageText;
    }

    private void UnSuscribeToPlayerViewEvents()
    {
        PlayerView.OnEnterInCookModeMessage -= ShowEnterMessageText;
        PlayerView.OnExitInCookModeMessage -= DisapearMessageText;
    }

    private void GetComponents()
    {
        cookMessageDesplayText = GetComponent<TextMeshProUGUI>();
    }

    private void InitializeReferencs()
    {
        cookMessageDesplayText.alignment = TextAlignmentOptions.Center;
    }

    private void ShowEnterMessageText()
    {
        string inputKey = "<color=yellow>'E'</color>";
        cookMessageDesplayText.text = "Presione la tecla " + inputKey + " para cocinar";
    }

    private void DisapearMessageText()
    {
        cookMessageDesplayText.text = "";
    }
}
