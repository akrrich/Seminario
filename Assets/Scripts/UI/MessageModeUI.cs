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
        UnsuscribeToPlayerViewEvents();
    }


    private void InitializeLamdaEventMessages()
    {
        //onCook += () => ShowEnterMessageText("Presione", "para entrar a cocinar");
        onAdministration += () => ShowEnterMessageText("Presione", "para entrar en administracion");
        //onHandOver += () => ShowEnterMessageText("Presione", "para entregar el plato");
        //onTakeOrder += () => ShowEnterMessageText("Presione", "para tomar el pedido");
        //onCleanDirtyTable += () => ShowEnterMessageText("Mantener", "para limpiar la mesa");
        //onThrowFoodToTrash += () => ShowEnterMessageText("Presione", "para tirar la comida a la basura");
    }

    private void SuscribeToPlayerViewEvents()
    {
        //PlayerView.OnCollisionEnterWithCookingDeskUIForCookModeMessage += onCook;
        //PlayerView.OnCollisionExitWithCookingDeskUIForCookModeMessage += DisapearMessageText;

        PlayerView.OnCollisionEnterWithAdministrationForAdministrationModeMessage += onAdministration;
        PlayerView.OnCollisionExitWithAdministrationForAdministrationModeMessage += DisapearMessageText;

        /*PlayerView.OnCollisionEnterWithTableForHandOverMessage += onHandOver;
        PlayerView.OnCollisionExitWithTableForHandOverMessage += DisapearMessageText;
        PlayerView.OnHandOverCompletedForHandOverMessage += DisapearMessageText;

        PlayerView.OnCollisionEnterWithTableForTakeOrderMessage += onTakeOrder;
        PlayerView.OnCollisionExitWithTableForTakeOrderMessage += DisapearMessageText;
        PlayerView.OnTakeOrderCompletedForHandOverMessage += DisapearMessageText;

        PlayerView.OnCollisionEnterWithTableForCleanDirtyTableMessage += onCleanDirtyTable;
        PlayerView.OnCollisionExitWithTableForCleanDirtyTableMessage += DisapearMessageText;

        PlayerView.OnCollisionEnterWithTrashForTrashModeMessage += onThrowFoodToTrash;
        PlayerView.OnCollisionExitWithTrashForTrashModeMessage += DisapearMessageText;*/
    }

    private void UnsuscribeToPlayerViewEvents()
    {
        //PlayerView.OnCollisionEnterWithCookingDeskUIForCookModeMessage -= onCook;
        //PlayerView.OnCollisionExitWithCookingDeskUIForCookModeMessage -= DisapearMessageText;

        PlayerView.OnCollisionEnterWithAdministrationForAdministrationModeMessage -= onAdministration;
        PlayerView.OnCollisionExitWithAdministrationForAdministrationModeMessage -= DisapearMessageText;

        /*PlayerView.OnCollisionEnterWithTableForHandOverMessage -= onHandOver;
        PlayerView.OnCollisionExitWithTableForHandOverMessage -= DisapearMessageText;
        PlayerView.OnHandOverCompletedForHandOverMessage -= DisapearMessageText;

        PlayerView.OnCollisionEnterWithTableForTakeOrderMessage -= onTakeOrder;
        PlayerView.OnCollisionExitWithTableForTakeOrderMessage -= DisapearMessageText;
        PlayerView.OnTakeOrderCompletedForHandOverMessage -= DisapearMessageText;

        PlayerView.OnCollisionEnterWithTableForCleanDirtyTableMessage -= onCleanDirtyTable;
        PlayerView.OnCollisionExitWithTableForCleanDirtyTableMessage -= DisapearMessageText;

        PlayerView.OnCollisionEnterWithTrashForTrashModeMessage -= onThrowFoodToTrash;
        PlayerView.OnCollisionExitWithTrashForTrashModeMessage -= DisapearMessageText;*/
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
