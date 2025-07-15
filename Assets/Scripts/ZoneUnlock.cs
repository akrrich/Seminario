using System.Collections.Generic;
using UnityEngine;

public class ZoneUnlock : MonoBehaviour
{
    [SerializeField] private ZoneUnlockData zoneUnlockData;
    [SerializeField] private List<GameObject> debris; // Provisorio

    [SerializeField] private List<Table> tablesToActive; // Provisorio
    private bool isUnlocked = false;

    public ZoneUnlockData ZoneUnlockData { get => zoneUnlockData; }

    public bool IsUnlocked { get => isUnlocked; set => isUnlocked = value; }


    public void UnlockZone()
    {
        foreach (var zone in debris) // Provisorio
        {
            zone.gameObject.SetActive(false);
        }

        foreach (var table in tablesToActive) // Provisorio
        {
            table.gameObject.SetActive(true);
        }
    }
}
