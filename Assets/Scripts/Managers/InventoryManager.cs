using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private List<GameObject> inventory = new List<GameObject>();


    public void SaveElementInInventory(GameObject element, Transform playerInvontory)
    {
        inventory.Add(element);

        element.transform.SetParent(playerInvontory);
        element.SetActive(false);
    }

    public void RemoveElmentFromInventory(Transform playerInvontor)
    {
        Transform child = playerInvontor.transform.GetChild(0);

        child.SetParent(null);
        child.gameObject.SetActive(true);

        inventory.Remove(child.gameObject);
    }
}
