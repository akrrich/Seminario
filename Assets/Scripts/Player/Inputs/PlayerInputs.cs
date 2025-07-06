using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : Singleton<PlayerInputs>
{
    [SerializeField] private InputsData keyboardInputs;
    [SerializeField] private InputsData joystickInputs;

    private PlayerInputActions inputActions; // Representa la clase creada por default del nuevo Inputsystem
    private Vector2 joystick = Vector2.zero;

    public InputsData KeyboardInputs { get => keyboardInputs; }
    public InputsData JoystickInputs { get => joystickInputs; }


    void Awake()
    {
        CreateSingleton(true);
        InitializePlayerInputActions();
    }

    void Update()
    {
        // Test JoystickButtons
        /*for (int i = 0; i < 20; i++) 
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton0 + i))
            {
                Debug.Log("Joystick button " + i + " presionado");
            }
        }*/
    }


    public Vector2 GetMoveAxis()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public Vector2 MouseRotation()
    {
        return new Vector2(Input.GetAxis("Mouse X") * keyboardInputs.SensitivityX, Input.GetAxis("Mouse Y") * keyboardInputs.SensitivityY);
    }

    public Vector2 JoystickRotation()
    {
        return new Vector2(joystick.x * joystickInputs.SensitivityX, joystick.y * joystickInputs.SensitivityY);
    }


    public bool Run() => Input.GetKeyDown(keyboardInputs.Run) || Input.GetKeyDown(joystickInputs.Run);
    public bool StopRun() => Input.GetKeyDown(keyboardInputs.Run) || Input.GetKeyDown(joystickInputs.Run);
    public bool GrabFood() => Input.GetKeyDown(keyboardInputs.GrabFood) || Input.GetKeyDown(joystickInputs.GrabFood);
    public bool HandOverFood() => Input.GetKeyDown(keyboardInputs.HandOverFood) || Input.GetKeyDown(joystickInputs.HandOverFood);
    public bool TakeClientOrder() => Input.GetKeyDown(keyboardInputs.TakeClientOrder) || Input.GetKeyDown(joystickInputs.TakeClientOrder);
    public bool CleanDirtyTable() => Input.GetKey(keyboardInputs.CleanDirtyTable) || Input.GetKey(joystickInputs.CleanDirtyTable);
    public bool Jump() => Input.GetKeyDown(keyboardInputs.Jump) || Input.GetKeyDown(joystickInputs.Jump);
    public bool Cook() => Input.GetKeyDown(keyboardInputs.Cook) || Input.GetKeyDown(joystickInputs.Cook);
    public bool Administration() => Input.GetKeyDown(keyboardInputs.Administration) || Input.GetKeyDown(joystickInputs.Administration);
    public bool ThrowFoodToTrash() => Input.GetKeyDown(keyboardInputs.ThrowFoodToTrash) || Input.GetKeyDown(joystickInputs.ThrowFoodToTrash);
    public bool Inventory() => Input.GetKeyDown(keyboardInputs.Inventory) || Input.GetKeyDown(joystickInputs.Inventory);
    public bool Pause() => Input.GetKeyDown(keyboardInputs.Pause) || Input.GetKeyDown(joystickInputs.Pause);

    public bool R1() => Input.GetKeyDown(KeyCode.Joystick1Button5);
    public bool L1() => Input.GetKeyDown(KeyCode.Joystick1Button4);

    public bool Attack() => Input.GetMouseButtonDown(0); //Click izquierdo
    public bool Dash() => Input.GetKeyDown(keyboardInputs.Dash);
    public bool RunHeld() => Input.GetKey(keyboardInputs.Run) || Input.GetKey(joystickInputs.Run);
    public bool Interact() => Input.GetKeyDown(keyboardInputs.Interact);


    private void InitializePlayerInputActions()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();

        inputActions.Player.Look.performed += ctx =>
        {
            joystick = ctx.ReadValue<Vector2>();
            if (ctx.control.device is Gamepad)
            {
                // Necesario verificar que no este en pausa para que cuando este pausado no afecta a la UI por el cursor
                if (PauseManager.Instance != null && !PauseManager.Instance.IsGamePaused)
                {
                    DeviceManager.Instance.CurrentDevice = Device.Joystick;
                }
            }
        };

        inputActions.Player.Look.canceled += ctx =>
        {
            joystick = Vector2.zero;
        };
    }
}
