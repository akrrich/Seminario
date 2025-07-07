using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropTable", menuName = "ScriptableObjects/Dungeon/New Monster Drop Table")]
public class DropTable : ScriptableObject
{
    public List<DropEntry> entries = new();

    public List<DropEntry> Roll()        // devolvemos directamente DropEntry
    {
        List<DropEntry> outList = new();
        foreach (var e in entries)
            if (Random.value <= e.probability)
                outList.Add(e);
        return outList;
    }
}
