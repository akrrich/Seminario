using TMPro;
using UnityEngine;
using System;

public class MessageModeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageDesplayText;

    private event Action onAdministration;


    void Awake()
    {
        InitializeLamdaEventMessages();
        SuscribeToPlayerViewEvents();
    }

    void Start()
    {
        InitializeReferencs();
    }

    void OnDestroy()
    {
        UnsuscribeToPlayerViewEvents();
    }


    private void InitializeLamdaEventMessages()
    {
        onAdministration += () => ShowEnterMessageText("Presione", "para entrar en administracion");
    }

    private void SuscribeToPlayerViewEvents()
    {
        PlayerView.OnCollisionEnterWithAdministrationForAdministrationModeMessage += onAdministration;
        PlayerView.OnCollisionExitWithAdministrationForAdministrationModeMessage += DisapearMessageText;
    }

    private void UnsuscribeToPlayerViewEvents()
    {
        PlayerView.OnCollisionEnterWithAdministrationForAdministrationModeMessage -= onAdministration;
        PlayerView.OnCollisionExitWithAdministrationForAdministrationModeMessage -= DisapearMessageText;
    }

    private void InitializeReferencs()
    {
        messageDesplayText.alignment = TextAlignmentOptions.Center;
    }

    private void ShowEnterMessageText(string actionText, string finalMessage)
    {
        string keyText = $"<color=yellow> {GetInteractInput()} </color>";
        messageDesplayText.text = $"{actionText} {keyText} {finalMessage}";
    }

    private void DisapearMessageText()
    {
        messageDesplayText.text = "";
    }

    private KeyCode GetInteractInput()
    {
        return DeviceManager.Instance.CurrentDevice == Device.Joystick
            ? PlayerInputs.Instance.JoystickInputs.Interact
            : PlayerInputs.Instance.KeyboardInputs.Interact;
    }
}
