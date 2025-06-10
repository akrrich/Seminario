using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private PlayerModel playerModel;

    private Vector3 cameraOffset;

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
        if (PauseManager.Instance != null && !playerModel.IsCooking && !playerModel.IsAdministrating && !PauseManager.Instance.IsGamePaused)
        {
            cameraOffset = new Vector3(0f, offSetY, 0.3f);

            float x, y;

            if (DeviceManager.Instance.CurrentDevice == Device.Joystick)
            {
                x = PlayerInputs.Instance.JoystickRotation().x * Time.deltaTime;
                y = PlayerInputs.Instance.JoystickRotation().y * Time.deltaTime;
            }

            else
            {
                x = PlayerInputs.Instance.MouseRotation().x * Time.deltaTime;
                y = PlayerInputs.Instance.MouseRotation().y * Time.deltaTime;
            }

            rotationX -= y;
            rotationX = Mathf.Clamp(rotationX, -80f, 80f);

            transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
            playerModel.transform.Rotate(Vector3.up * x);
        }
    }
}
