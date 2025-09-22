using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffect : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float bobSpeed = 1f;
    [SerializeField] private float bobHeight = 0.5f;

    private Vector3 startPosition;
    private float bobTime = 0f;

    void FixedUpdate()
    {
        // Rotar el item
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Efecto de flotación
        bobTime += Time.deltaTime * bobSpeed;
        float newY = startPosition.y + Mathf.Sin(bobTime) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
