using TMPro;
using UnityEngine;
using System;

public class MessageModeUI : MonoBehaviour
{
    private TextMeshProUGUI messageDesplayText;

    private Action onCook, onAdministration, onHandOver;


    void Awake()
    {
        GetComponents();
        InitializeMessages();
        SuscribeToPlayerViewEvents();
    }

    void Start()
    {
        InitializeReferencs();
    }

    void OnDestroy()
    {
        UnSuscribeToPlayerViewEvents();
    }


    private void InitializeMessages()
    {
        onCook += () => ShowEnterMessageText(" para entrar a cocinar");
        onAdministration += () => ShowEnterMessageText(" para entrar en administracion");
        onHandOver += () => ShowEnterMessageText(" para entregar el plato");
    }

    private void SuscribeToPlayerViewEvents()
    {
        PlayerView.OnCollisionEnterWithOvenForCookModeMessage += onCook;
        PlayerView.OnCollisionExitWithOvenForCookModeMessage += DisapearMessageText;

        PlayerView.OnCollisionEnterWithAdministrationForAdministrationModeMessage += onAdministration;
        PlayerView.OnCollisionExitWithAdministrationForAdministrationModeMessage += DisapearMessageText;

        PlayerView.OnCollisionEnterWithTableForHandOverMessage += onHandOver;
        PlayerView.OnCollisionExitWithTableForHandOverMessage += DisapearMessageText;
        PlayerView.OnHandOverCompletedForHandOverMessage += DisapearMessageText;
    }

    private void UnSuscribeToPlayerViewEvents()
    {
        PlayerView.OnCollisionEnterWithOvenForCookModeMessage -= onCook;
        PlayerView.OnCollisionExitWithOvenForCookModeMessage -= DisapearMessageText;

        PlayerView.OnCollisionEnterWithAdministrationForAdministrationModeMessage -= onAdministration;
        PlayerView.OnCollisionExitWithAdministrationForAdministrationModeMessage -= DisapearMessageText;

        PlayerView.OnCollisionEnterWithTableForHandOverMessage -= onHandOver;
        PlayerView.OnCollisionExitWithTableForHandOverMessage -= DisapearMessageText;
        PlayerView.OnHandOverCompletedForHandOverMessage -= DisapearMessageText;
    }

    private void GetComponents()
    {
        messageDesplayText = GetComponent<TextMeshProUGUI>();
    }

    private void InitializeReferencs()
    {
        messageDesplayText.alignment = TextAlignmentOptions.Center;
    }

    private void ShowEnterMessageText(string finalMessage)
    {
        string inputKey = "<color=yellow>'E'</color>";

        messageDesplayText.text = "Presione la tecla " + inputKey + finalMessage;
    }

    private void DisapearMessageText()
    {
        messageDesplayText.text = "";
    }
}
