using UnityEngine;

[CreateAssetMenu(fileName = "DeviceManagerData", menuName = "ScriptableObjects/Create New DeviceManagerData")]
public class DeviceManagerData : ScriptableObject
{
    [SerializeField] private bool useCursorAllTime;

    public bool UseCursorAllTime { get => useCursorAllTime; }
}
