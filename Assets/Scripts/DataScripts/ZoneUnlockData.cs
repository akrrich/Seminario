using UnityEngine;

[CreateAssetMenu(fileName = "ZoneUnlockData", menuName = "ScriptableObjects/Create New ZoneUnlockData")]
public class ZoneUnlockData : ScriptableObject
{
    [SerializeField] private Sprite imageZoneUnlock;
    [SerializeField] private int cost;

    public Sprite ImageZoneUnlock { get => imageZoneUnlock; }
    public int Cost { get => cost; }
}