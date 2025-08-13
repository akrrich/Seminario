using UnityEngine;

[CreateAssetMenu(fileName = "InteractionManagerData", menuName = "ScriptableObjects/Create New InteractionManagerData")]
public class InteractionManagerData : ScriptableObject
{
    [SerializeField] private float interactionDistance;

    public float InteractionDistance { get => interactionDistance; }
}
