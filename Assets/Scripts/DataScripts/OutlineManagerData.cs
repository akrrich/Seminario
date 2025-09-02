using UnityEngine;

[CreateAssetMenu(fileName = "OutlineManager", menuName = "ScriptableObjects/Create New OutlineManager")]
public class OutlineManagerData : ScriptableObject
{
    [SerializeField] private Color defaultOutlineColor;

    [Range(0.1f, 10f)]
    [SerializeField] private float activeWidth;

    public Color DefaultOutlineColor { get =>  defaultOutlineColor; }

    public float ActiveWidth { get => activeWidth; }
}
