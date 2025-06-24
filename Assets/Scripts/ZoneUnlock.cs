using UnityEngine;

public class ZoneUnlock : MonoBehaviour
{
    [SerializeField] private Sprite imageZoneUnlock;
    [SerializeField] private int cost;

    public Sprite ImageZoneUnlock { get => imageZoneUnlock; }
    public int Cost { get => cost; }


    public void UnlockZone()
    {
        gameObject.SetActive(false);
    }
}
