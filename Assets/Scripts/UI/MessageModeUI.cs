using TMPro;
using UnityEngine;
using System;

public class MessageModeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageDesplayText;

    private event Action onCook, onAdministration, onHandOver, onTakeOrder;


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
        UnSuscribeToPlayerViewEvents();
    }


    private void InitializeLamdaEventMessages()
    {
        onCook += () => ShowEnterMessageText(" para entrar a cocinar", GetCookKey());
        onAdministration += () => ShowEnterMessageText(" para entrar en administracion", GetAdministrationKey());
        onHandOver += () => ShowEnterMessageText(" para entregar el plato", GetHandOverKey());
        onTakeOrder += () => ShowEnterMessageText(" para tomar el pedido", GetTakeOrderKey());
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

        PlayerView.OnCollisionEnterWithTableForTakeOrderMessage += onTakeOrder;
        PlayerView.OnCollisionExitWithTableForTakeOrderMessage += DisapearMessageText;
        PlayerView.OnTakeOrderCompletedForHandOverMessage += DisapearMessageText;
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

        PlayerView.OnCollisionEnterWithTableForTakeOrderMessage -= onTakeOrder;
        PlayerView.OnCollisionExitWithTableForTakeOrderMessage -= DisapearMessageText;
        PlayerView.OnTakeOrderCompletedForHandOverMessage -= DisapearMessageText;
    }

    private void InitializeReferencs()
    {
        messageDesplayText.alignment = TextAlignmentOptions.Center;
    }

    private void ShowEnterMessageText(string finalMessage, KeyCode inputKey)
    {
        string keyText = $"<color=yellow> {inputKey} </color>";
        messageDesplayText.text = "Presione" + keyText + finalMessage;
    }

    private void DisapearMessageText()
    {
        messageDesplayText.text = "";
    }

    private KeyCode GetCookKey()
    {
        return DeviceManager.Instance.CurrentDevice == Device.Joystick
            ? PlayerInputs.Instance.JoystickInputs.Cook
            : PlayerInputs.Instance.KeyboardInputs.Cook;
    }

    private KeyCode GetAdministrationKey()
    {
        return DeviceManager.Instance.CurrentDevice == Device.Joystick
            ? PlayerInputs.Instance.JoystickInputs.Administration
            : PlayerInputs.Instance.KeyboardInputs.Administration;
    }

    private KeyCode GetHandOverKey()
    {
        return DeviceManager.Instance.CurrentDevice == Device.Joystick
            ? PlayerInputs.Instance.JoystickInputs.HandOverFood
            : PlayerInputs.Instance.KeyboardInputs.HandOverFood;
    }

    private KeyCode GetTakeOrderKey()
    {
        return DeviceManager.Instance.CurrentDevice == Device.Joystick
            ? PlayerInputs.Instance.JoystickInputs.TakeClientOrder
            : PlayerInputs.Instance.KeyboardInputs.TakeClientOrder;
    }
}
