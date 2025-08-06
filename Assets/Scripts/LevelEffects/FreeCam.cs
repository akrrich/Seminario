using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCam : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 3f;

    void Update()
    {
        // Camera movement
        float moveForward = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float moveSide = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveUp = 0f;

        if (Input.GetKey(KeyCode.E))
        {
            moveUp += moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            moveUp -= moveSpeed * Time.deltaTime;
        }

        transform.Translate(new Vector3(moveSide, moveUp, moveForward));

        // Camera rotation (spectator mode)
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            float rotY = Input.GetAxis("Mouse X") * rotationSpeed;
            float rotX = -Input.GetAxis("Mouse Y") * rotationSpeed;

            transform.eulerAngles += new Vector3(rotX, rotY, 0f);
        }
    }
}
