using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTest : MonoBehaviour,IInteractable
{
    public void Interact()
    {
        Debug.Log("Interactuaste");
        Destroy(gameObject);
    }
}
