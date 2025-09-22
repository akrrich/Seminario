using UnityEngine;

[CreateAssetMenu(fileName = "SaveSystemData", menuName = "ScriptableObjects/Create New SaveSystemData")]
public class SaveSystemData : ScriptableObject
{
    [Header("Define si se utiliza el sistema de guardado al correr el juego")]
    [SerializeField] private bool useSaveSystem;

    public bool UseSaveSystem { get => useSaveSystem; }
}
