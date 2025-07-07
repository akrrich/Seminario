using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropTable", menuName = "ScriptableObjects/Dungeon/New Monster Drop Table")]
public class DropTable : ScriptableObject
{
    public List<DropEntry> entries;
    public DropEntry Roll()
    {
        Dictionary<DropEntry, float> lootProbabilities = new Dictionary<DropEntry, float>();
        foreach (var entry in entries)
        {
            lootProbabilities[entry] = entry.probability;
        }
        return RouletteSelection.Roulette(lootProbabilities);
    }
}
