using UnityEngine;

public class ZoneUnlock : MonoBehaviour
{
    [SerializeField] private Sprite imageZoneUnlock;
    [SerializeField] private int cost;

    private bool isUnlocked = false;

    public Sprite ImageZoneUnlock { get => imageZoneUnlock; }
    public int Cost { get => cost; }
    public bool IsUnlocked { get => isUnlocked; set => isUnlocked = value; }


    public void UnlockZone()
    {
        gameObject.SetActive(false);
    }
}
