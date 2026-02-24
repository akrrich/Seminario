using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Device
{
    KeyboardMouse, Joystick
}
public enum UIModeSource
{
    Legacy,
    Pause,
    Administration,
    Cooking,
    TrashPanel,
    Tutorial,
    ResumeDay
}
public class DeviceManager : Singleton<DeviceManager>
{
    [Header("Config")]
    [SerializeField] private DeviceManagerData deviceManagerData;
    private Device currentDevice;

    private HashSet<UIModeSource> uiModeSources = new HashSet<UIModeSource>();

    private bool isUIModeActive;
    public bool IsUIModeActive
    {
        get => isUIModeActive;
        set
        {
            if (value)
                AddUIModeSource(UIModeSource.Legacy);
            else
                RemoveUIModeSource(UIModeSource.Legacy);
        }
    }
    public Device CurrentDevice { get => currentDevice; set => currentDevice = value; }
    void Awake()
    {
        CreateSingleton(true);
        SubscribeToUpdateManager();
    }
    void OnDestroy()
    {
        UnsubscribeFromUpdateManager();
    }
    // Simulacion de Update
    void UpdateDeviceManager()
    {
        DetectDevice();
        RecalculateUIMode();
        ApplyCursorState();
    }


    private void SubscribeToUpdateManager()
    {
        UpdateManager.OnUpdateAllTime += UpdateDeviceManager;
    }

    private void UnsubscribeFromUpdateManager()
    {
        UpdateManager.OnUpdateAllTime -= UpdateDeviceManager;
    }
    public void AddUIModeSource(UIModeSource source)
    {
        if (uiModeSources.Add(source))
        {
            // Solo recalcula si realmente cambió algo
            RecalculateUIMode();
        }
    }

    public void RemoveUIModeSource(UIModeSource source)
    {
        if (uiModeSources.Remove(source))
        {
            RecalculateUIMode();
        }
    }

    private void RecalculateUIMode()
    {
        isUIModeActive = uiModeSources.Count > 0;
    }

    private void ApplyCursorState()
    {
        if (deviceManagerData != null && deviceManagerData.UseCursorAllTime)
            return;

        if (isUIModeActive)
        {
            if (!Cursor.visible || Cursor.lockState != CursorLockMode.None)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else
        {
            if (Cursor.visible || Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    private void DetectDevice()
    {
        // Detectar Gamepad (nuevo Input System)
        if (Gamepad.current != null)
        {
            if (Gamepad.current.wasUpdatedThisFrame)
            {
                currentDevice = Device.Joystick;
                return;
            }
        }

        // Detectar Mouse movimiento
        if (Mouse.current != null)
        {
            if (Mouse.current.delta.ReadValue() != Vector2.zero ||
                Mouse.current.leftButton.wasPressedThisFrame)
            {
                currentDevice = Device.KeyboardMouse;
                return;
            }
        }

        // Detectar teclado
        if (Keyboard.current != null)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                currentDevice = Device.KeyboardMouse;
            }
        }
    }

}
