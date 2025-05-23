using System;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private static Action onCollisionEnterWithOvenForCookModeMessage; // Mostrar el texto de la UI
    private static Action onCollisionExitWithOvenForCookModeMessage; // Esconder el texto de la UI
    private static Action onEnterInCookMode; // Entrar en el modo cocinar para mostrar la UI de cocina
    private static Action onExitInCookMode; // Salir del modo cocinar para ocultar la UI de cocina

    private static Action onCollisionEnterWithAdministrationForAdministrationModeMessage; // Mostrar el texto de la UI
    private static Action onCollisionExitWithAdministrationForAdministrationModeMessage; // Esconder el texto de la UI
    private static Action onEnterInAdministrationMode; // Entrar en el modo administracion para mostrar la UI de administracion
    private static Action onExitInAdministrationMode; // Salir del modo administracion para ocultar la UI de administracion

    private static Action onCollisionEnterWithTableForHandOverMessage; // Mostrar el texto de la UI
    private static Action onCollisionExitWithTableForHandOverMessage; // Esconder el texto de la UI
    private static Action onHandOverCompletedForHandOverMessage; // Esconder el texto de la UI

    private static Action onCollisionEnterWithTableForTakeOrderMessage; // Mostrar el texto de la UI
    private static Action onCollisionExitWithTableForTakeOrderMessage; // Esconder el texto de la UI
    private static Action onTakeOrderCompletedForHandOverMessage; // Esconder el texto de la UI

    private static Action onDeactivateInventoryFoodUI; // Esconder la UI del inventario


    public static Action OnCollisionEnterWithOvenForCookModeMessage { get => onCollisionEnterWithOvenForCookModeMessage; set => onCollisionEnterWithOvenForCookModeMessage = value; }
    public static Action OnCollisionExitWithOvenForCookModeMessage { get => onCollisionExitWithOvenForCookModeMessage; set => onCollisionExitWithOvenForCookModeMessage = value; }    
    public static Action OnEnterInCookMode { get => onEnterInCookMode; set => onEnterInCookMode = value; }
    public static Action OnExitInCookMode { get => onExitInCookMode; set => onExitInCookMode = value; }
    
    public static Action OnCollisionEnterWithAdministrationForAdministrationModeMessage { get => onCollisionEnterWithAdministrationForAdministrationModeMessage; set => onCollisionEnterWithAdministrationForAdministrationModeMessage = value;}
    public static Action OnCollisionExitWithAdministrationForAdministrationModeMessage { get => onCollisionExitWithAdministrationForAdministrationModeMessage; set => onCollisionExitWithAdministrationForAdministrationModeMessage = value; }
    public static Action OnEnterInAdministrationMode { get => onEnterInAdministrationMode; set => onEnterInAdministrationMode = value;}
    public static Action OnExitInAdministrationMode { get => onExitInAdministrationMode; set => onExitInAdministrationMode = value;}    

    public static Action OnCollisionEnterWithTableForHandOverMessage { get => onCollisionEnterWithTableForHandOverMessage; set => onCollisionEnterWithTableForHandOverMessage = value; }
    public static Action OnCollisionExitWithTableForHandOverMessage { get => onCollisionExitWithTableForHandOverMessage; set => onCollisionExitWithTableForHandOverMessage = value; }
    public static Action OnHandOverCompletedForHandOverMessage { get => onHandOverCompletedForHandOverMessage; set => onHandOverCompletedForHandOverMessage = value; }

    public static Action OnCollisionEnterWithTableForTakeOrderMessage { get => onCollisionEnterWithTableForTakeOrderMessage; set => onCollisionEnterWithTableForTakeOrderMessage = value; }
    public static Action OnCollisionExitWithTableForTakeOrderMessage { get => onCollisionExitWithTableForTakeOrderMessage; set => onCollisionExitWithTableForTakeOrderMessage = value; }
    public static Action OnTakeOrderCompletedForHandOverMessage { get => onTakeOrderCompletedForHandOverMessage; set => onTakeOrderCompletedForHandOverMessage = value; }

    public static Action OnDeactivateInventoryFoodUI { get => onDeactivateInventoryFoodUI; set => onDeactivateInventoryFoodUI = value; }

}
