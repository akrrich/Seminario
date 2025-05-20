using UnityEngine;
using UnityEngine.InputSystem;

public enum Device
{
    KeyboardMouse, Joystick
}

public class DeviceManager : MonoBehaviour
{
    private static DeviceManager instance;

    private Device currentDevice;

    private bool isUIModeActive = false; // Falso por defecto, se inicializa en ScenesManager y se setea cuando se quiere interactuar en la UI

    public static DeviceManager Instance { get => instance; }

    public Device CurrentDevice { get => currentDevice; set => currentDevice = value; }

    public bool IsUIModeActive { get => isUIModeActive; set => isUIModeActive = value; }


    void Awake()
    {
        CreateSingleton();
    }

    void Update()
    {
        IsJoystickUsed();
        IsMouseAndKeyboardUsed();
        EnabledAndDisabledCursor();

        print(isUIModeActive);
    }


    private void CreateSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void EnabledAndDisabledCursor()
    {
        if (currentDevice == Device.Joystick)
        {
            Cursor.visible = false;    
        }

        else if (currentDevice == Device.KeyboardMouse)
        {
            if (isUIModeActive)
            {
                Cursor.visible = true;
            }

            else
            {
                Cursor.visible = false;
            }
        }
    }

    private void IsJoystickUsed()
    {
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKey((KeyCode)((int)KeyCode.JoystickButton0 + i)))
            {
                currentDevice = Device.Joystick;
                return;
            }
        }

        Gamepad gamepad = Gamepad.current;
        if (gamepad != null)
        {
            if (gamepad.buttonSouth.isPressed || gamepad.leftStick.ReadValue().magnitude > 0.1f ||
                gamepad.dpad.up.isPressed || gamepad.dpad.down.isPressed ||
                gamepad.dpad.left.isPressed || gamepad.dpad.right.isPressed)
            {
                currentDevice = Device.Joystick;
                return;
            }
        }

        if (Mathf.Abs(Input.GetAxis("Joystick Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Joystick Vertical")) > 0.1f)
        {
            currentDevice = Device.Joystick;
            return;
        }
    }

    private void IsMouseAndKeyboardUsed()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            currentDevice = Device.KeyboardMouse;
            return;
        }

        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (key >= KeyCode.A && key <= KeyCode.Z ||        // Letras
                key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9 || // Números superiores
                key >= KeyCode.Keypad0 && key <= KeyCode.Keypad9 || // Números del teclado numérico
                key >= KeyCode.F1 && key <= KeyCode.F12 || // Teclas de función
                key == KeyCode.Space || key == KeyCode.Return || key == KeyCode.Backspace || // Espacio, Enter, Borrar
                key == KeyCode.Tab || key == KeyCode.Escape || key == KeyCode.LeftControl || key == KeyCode.RightControl ||
                key == KeyCode.LeftShift || key == KeyCode.RightShift || key == KeyCode.LeftAlt || key == KeyCode.RightAlt ||
                key == KeyCode.UpArrow || key == KeyCode.DownArrow || key == KeyCode.LeftArrow || key == KeyCode.RightArrow || // Flechas
                key == KeyCode.BackQuote) // Tecla al lado del 1 (tilde)
            {
                if (Input.GetKeyDown(key))
                {
                    currentDevice = Device.KeyboardMouse;
                    return;
                }
            }
        }
    }
}
