using TMPro;
using UnityEngine;
using System;

public class MessageModeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageDesplayText;

    private event Action onCook, onAdministration, onHandOver, onTakeOrder, onCleanDirtyTable, onThrowFoodToTrash;


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
        onThrowFoodToTrash += () => ShowEnterMessageText("Presione", GetThrowFoodToTrashKey(), "para tirar la comida a la basura");
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

        PlayerView.OnCollisionEnterWithTrashForTrashModeMessage += onThrowFoodToTrash;
        PlayerView.OnCollisionExitWithTrashForTrashModeMessage += DisapearMessageText;
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

        PlayerView.OnCollisionEnterWithTrashForTrashModeMessage -= onThrowFoodToTrash;
        PlayerView.OnCollisionExitWithTrashForTrashModeMessage -= DisapearMessageText;
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
            ? PlayerInputs.Instance.JoystickInputs.Interact
            : PlayerInputs.Instance.KeyboardInputs.Interact;
    }

    private KeyCode GetAdministrationKey()
    {
        return DeviceManager.Instance.CurrentDevice == Device.Joystick
            ? PlayerInputs.Instance.JoystickInputs.Interact
            : PlayerInputs.Instance.KeyboardInputs.Interact;
    }

    private KeyCode GetHandOverKey()
    {
        return DeviceManager.Instance.CurrentDevice == Device.Joystick
            ? PlayerInputs.Instance.JoystickInputs.Interact
            : PlayerInputs.Instance.KeyboardInputs.Interact;
    }

    private KeyCode GetTakeOrderKey()
    {
        return DeviceManager.Instance.CurrentDevice == Device.Joystick
            ? PlayerInputs.Instance.JoystickInputs.Interact
            : PlayerInputs.Instance.KeyboardInputs.Interact;
    }

    private KeyCode GetCleanDirtyTableKey()
    {
        return DeviceManager.Instance.CurrentDevice == Device.Joystick
            ? PlayerInputs.Instance.JoystickInputs.Interact
            : PlayerInputs.Instance.KeyboardInputs.Interact;
    }

    private KeyCode GetThrowFoodToTrashKey()
    {
        return DeviceManager.Instance.CurrentDevice == Device.Joystick
            ? PlayerInputs.Instance.JoystickInputs.Interact
            : PlayerInputs.Instance.KeyboardInputs.Interact;
    }
}
