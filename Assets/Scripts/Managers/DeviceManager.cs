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

    private bool isUIActive = false; // Falso por defecto, se inicializa en ScenesManager y se setea cuando se quiere interactuar en la UI

    public static DeviceManager Instance { get => instance; }

    public Device CurrentDevice { get => currentDevice; set => currentDevice = value; }

    public bool IsUIActive { get => isUIActive; set => isUIActive = value; }


    void Awake()
    {
        CreateSingleton();
    }

    void Update()
    {
        EnabledAndDisabledCursor();
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
        if (Time.timeScale == 0f || isUIActive)
        {
            IsJoystickUsed();
            IsMouseAndKeyboardUsed();
        }

        else
        {
            Cursor.visible = false;
        }        
    }

    private void IsJoystickUsed()
    {
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKey((KeyCode)((int)KeyCode.JoystickButton0 + i)))
            {
                currentDevice = Device.Joystick;
                Cursor.visible = false;
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
                Cursor.visible = false;
                return;
            }
        }

        if (Mathf.Abs(Input.GetAxis("Joystick Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Joystick Vertical")) > 0.1f)
        {
            currentDevice = Device.Joystick;
            Cursor.visible = false;
            return;
        }
    }

    private void IsMouseAndKeyboardUsed()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            currentDevice = Device.KeyboardMouse;
            Cursor.visible = true;
            return;
        }

        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (key >= KeyCode.A && key <= KeyCode.Z ||        // Letras
                key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9 || // N�meros superiores
                key >= KeyCode.Keypad0 && key <= KeyCode.Keypad9 || // N�meros del teclado num�rico
                key >= KeyCode.F1 && key <= KeyCode.F12 || // Teclas de funci�n
                key == KeyCode.Space || key == KeyCode.Return || key == KeyCode.Backspace || // Espacio, Enter, Borrar
                key == KeyCode.Tab || key == KeyCode.Escape || key == KeyCode.LeftControl || key == KeyCode.RightControl ||
                key == KeyCode.LeftShift || key == KeyCode.RightShift || key == KeyCode.LeftAlt || key == KeyCode.RightAlt ||
                key == KeyCode.UpArrow || key == KeyCode.DownArrow || key == KeyCode.LeftArrow || key == KeyCode.RightArrow || // Flechas
                key == KeyCode.BackQuote) // Tecla al lado del 1 (tilde)
            {
                if (Input.GetKeyDown(key))
                {
                    currentDevice = Device.KeyboardMouse;
                    Cursor.visible = true;
                    return;
                }
            }
        }
    }
}
