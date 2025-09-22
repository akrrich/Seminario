using UnityEngine;

[CreateAssetMenu(fileName = "DeviceManagerData", menuName = "ScriptableObjects/Create New DeviceManagerData")]
public class DeviceManagerData : ScriptableObject
{
    [Header("Define si se muestra el cursor dentro del juego todo el tiempo o solamente cuando corresponde")]
    [SerializeField] private bool useCursorAllTime;

    public bool UseCursorAllTime { get => useCursorAllTime; }
}
