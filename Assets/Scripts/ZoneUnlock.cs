using UnityEngine;

public class ZoneUnlock : MonoBehaviour
{
    [SerializeField] private ZoneUnlockData zoneUnlockData;

    private bool isUnlocked = false;

    public ZoneUnlockData ZoneUnlockData { get => zoneUnlockData; }

    public bool IsUnlocked { get => isUnlocked; set => isUnlocked = value; }


    public void UnlockZone()
    {
        gameObject.SetActive(false);
    }
}
