using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour,IInteractable
{
    private int hitPoints = 1;

    public void Interact()
    {
        TakeHit();
    }

    public void TakeHit()
    {
        hitPoints--;
        if (hitPoints <= 0)
        {
            BreakBarrel();
        }
    }

    private void BreakBarrel()
    {
        float roll = Random.value;

        if (roll < 0.5f) Debug.Log("Ingrediente común");
        else if (roll < 0.7f) Debug.Log("Ingrediente raro");
        else Debug.Log("Nada");

        Destroy(gameObject);
    }
}
