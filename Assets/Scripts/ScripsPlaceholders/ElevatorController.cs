using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
   public Transform platform;         // La plataforma que sube y baja
    public Transform upperPoint;       // Punto de destino arriba
    public Transform lowerPoint;       // Punto de destino abajo
    public float speed = 2f;

    private bool playerOnElevator = false;

    void Update()
    {
        Vector3 targetPosition = playerOnElevator ? upperPoint.position : lowerPoint.position;
        platform.position = Vector3.MoveTowards(platform.position, targetPosition, speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnElevator = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnElevator = false;
        }
    }
}
