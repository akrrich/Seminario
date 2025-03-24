using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private PlayerModel playerModel;

    private Vector3 cameraOffset;

    private float sensitivity = 2f;
    private float rotationX = 0f;

    private float offSetY = 1f; // Posicion de la camara en eje y


    void Awake()
    {
        GetComponents();
        InitializeCameraPosition();
    }

    void Update()
    {
        UpdateCameraFollow();
    }


    private void GetComponents()
    {
        playerModel = GetComponentInParent<PlayerModel>();
    }

    private void InitializeCameraPosition()
    {
        cameraOffset = new Vector3(0f, offSetY, 0.3f);
        transform.position = playerModel.transform.position + cameraOffset;
    }

    private void UpdateCameraFollow()
    {
        if (!playerModel.IsCooking)
        {
            cameraOffset = new Vector3(0f, offSetY, 0.3f);

            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -80f, 80f);

            transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

            playerModel.transform.Rotate(Vector3.up * mouseX);
        }
    }
}
