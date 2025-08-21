using UnityEngine;

[CreateAssetMenu(fileName = "InteractionManagerUIData", menuName = "ScriptableObjects/Create New InteractionManagerUIData")]
public class InteractionManagerUIData : ScriptableObject
{
    [SerializeField] private Color normalColor;
    [SerializeField] private Color interactiveColor;

    public Color NormalColor { get => normalColor; }
    public Color InteractiveColor { get => interactiveColor; }
}
