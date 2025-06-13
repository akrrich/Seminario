using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float _xRotation;
    float _yRotation;

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
       // Cursor.visible = false; 
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        _yRotation += mouseX;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        // Cámara rota con X e Y (para mirar)
        transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);

        // Orientation solo rota en Y (para movimiento)
        if (orientation != null)
            orientation.rotation = Quaternion.Euler(0f, _yRotation, 0f);
    }
}
