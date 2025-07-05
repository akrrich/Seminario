using TMPro;
using UnityEngine;
using System;

public class MessageModeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageDesplayText;

    private event Action onCook, onAdministration, onHandOver, onTakeOrder, onCleanDirtyTable;


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
        onCook += () => ShowEnterMessageText("Presione", GetCookKey(), "para entrar a cocinar");
        onAdministration += () => ShowEnterMessageText("Presione", GetAdministrationKey(), "para entrar en administracion");
        onHandOver += () => ShowEnterMessageText("Presione", GetHandOverKey(), "para entregar el plato");
        onTakeOrder += () => ShowEnterMessageText("Presione", GetTakeOrderKey(), "para tomar el pedido");
        onCleanDirtyTable += () => ShowEnterMessageText("Mantener", GetCleanDirtyTableKey(), "para limpiar la mesa");
    }

    private void SuscribeToPlayerViewEvents()
    {
        PlayerView.OnCollisionEnterWithCookingDeskUIForCookModeMessage += onCook;
        PlayerView.OnCollisionExitWithCookingDeskUIForCookModeMessage += DisapearMessageText;

        PlayerView.OnCollisionEnterWithAdministrationForAdministrationModeMessage += onAdministration;
        PlayerView.OnCollisionExitWithAdministrationForAdministrationModeMessage += DisapearMessageText;

        PlayerView.OnCollisionEnterWithTableForHandOverMessage += onHandOver;
        PlayerView.OnCollisionExitWithTableForHandOverMessage += DisapearMessageText;
        PlayerView.OnHandOverCompletedForHandOverMessage += DisapearMessageText;

        PlayerView.OnCollisionEnterWithTableForTakeOrderMessage += onTakeOrder;
        PlayerView.OnCollisionExitWithTableForTakeOrderMessage += DisapearMessageText;
        PlayerView.OnTakeOrderCompletedForHandOverMessage += DisapearMessageText;

        PlayerView.OnCollisionEnterWithTableForCleanDirtyTableMessage += onCleanDirtyTable;
        PlayerView.OnCollisionExitWithTableForCleanDirtyTableMessage += DisapearMessageText;
    }

    private void UnSuscribeToPlayerViewEvents()
    {
        PlayerView.OnCollisionEnterWithCookingDeskUIForCookModeMessage -= onCook;
        PlayerView.OnCollisionExitWithCookingDeskUIForCookModeMessage -= DisapearMessageText;

        PlayerView.OnCollisionEnterWithAdministrationForAdministrationModeMessage -= onAdministration;
        PlayerView.OnCollisionExitWithAdministrationForAdministrationModeMessage -= DisapearMessageText;

        PlayerView.OnCollisionEnterWithTableForHandOverMessage -= onHandOver;
        PlayerView.OnCollisionExitWithTableForHandOverMessage -= DisapearMessageText;
        PlayerView.OnHandOverCompletedForHandOverMessage -= DisapearMessageText;

        PlayerView.OnCollisionEnterWithTableForTakeOrderMessage -= onTakeOrder;
        PlayerView.OnCollisionExitWithTableForTakeOrderMessage -= DisapearMessageText;
        PlayerView.OnTakeOrderCompletedForHandOverMessage -= DisapearMessageText;

        PlayerView.OnCollisionEnterWithTableForCleanDirtyTableMessage -= onCleanDirtyTable;
        PlayerView.OnCollisionExitWithTableForCleanDirtyTableMessage -= DisapearMessageText;
    }

    private void InitializeReferencs()
    {
        messageDesplayText.alignment = TextAlignmentOptions.Center;
    }

    private void ShowEnterMessageText(string actionText, KeyCode inputKey, string finalMessage)
    {
        string keyText = $"<color=yellow> {inputKey} </color>";
        messageDesplayText.text = $"{actionText} {keyText} {finalMessage}";
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

    private KeyCode GetCleanDirtyTableKey()
    {
        return DeviceManager.Instance.CurrentDevice == Device.Joystick
            ? PlayerInputs.Instance.JoystickInputs.CleanDirtyTable
            : PlayerInputs.Instance.KeyboardInputs.CleanDirtyTable;
    }
}
