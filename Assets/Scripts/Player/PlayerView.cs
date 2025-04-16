using System;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private static Action onCollisionEnterWithOvenForCookModeMessage; // Mostrar el texto de la UI
    private static Action onCollisionExitWithOvenForCookModeMessage; // Esconder el texto de la UI
    private static Action onEnterInCookMode; // Entrar en el modo cocinar para mostrar la UI
    private static Action onExitInCookMode; // Salir del modo cocinar para ocultar la UI

    public static Action OnEnterInCookModeMessage { get => onCollisionEnterWithOvenForCookModeMessage; set => onCollisionEnterWithOvenForCookModeMessage = value; }
    public static Action OnExitInCookModeMessage { get => onCollisionExitWithOvenForCookModeMessage; set => onCollisionExitWithOvenForCookModeMessage = value; }
    public static Action OnEnterInCookMode { get => onEnterInCookMode; set => onEnterInCookMode = value; }
    public static Action OnExitInCookMode { get => onExitInCookMode; set => onExitInCookMode= value; }
}
