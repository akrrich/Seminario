
using UnityEngine;

[System.Serializable]
public class DropEntry
{
    public string lootName;              // “Carne de Bestia”, “Dientes”, etc.
    public int minAmount = 1;
    public int maxAmount = 1;
    [Range(0f, 1f)] public float probability = 1f;
}
