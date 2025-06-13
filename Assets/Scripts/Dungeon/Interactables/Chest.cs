using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour,IInteractable
{
    private bool isOpened = false;

    public void Interact()
    {
        if (isOpened) return;
        isOpened = true;
        OpenChest();
    }

    private void OpenChest()
    {
        float roll = Random.value;

        if (roll < 0.4f) Debug.Log("Ingrediente raro");
        else if (roll < 0.6f) Debug.Log("50 Dientes");
        else Debug.Log("Receta desbloqueada");

        // Animación opcional
        // UI para apertura y el invnetario del cofre.
    }
}
