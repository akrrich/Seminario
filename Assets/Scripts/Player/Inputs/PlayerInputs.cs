using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    private static PlayerInputs instance;

    [SerializeField] private Inputs keyboardInputs;
    [Header("")]
    [SerializeField] private Inputs joystickInputs;

    private PlayerInputActions inputActions; // Representa la clase creada por default del nuevo Inputsystem
    private Vector2 joystick = Vector2.zero;

    public static PlayerInputs Instance { get => instance; }

    public Inputs KeyboardInputs { get => keyboardInputs; }
    public Inputs JoystickInputs { get => joystickInputs; }


    void Awake()
    {
        CreateSingleton();
        InitializePlayerInputActions();
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
    
    public bool Inventory() => Input.GetKeyDown(keyboardInputs.Inventory) || Input.GetKeyDown(joystickInputs.Inventory);
    
    public bool Pause() => Input.GetKeyDown(keyboardInputs.Pause) || Input.GetKeyDown(joystickInputs.Pause);


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

[System.Serializable]
public class Inputs
{
    [Header("Inputs:")]
    [SerializeField] private KeyCode run;
    [SerializeField] private KeyCode grabFood;
    [SerializeField] private KeyCode handOverFood;
    [SerializeField] private KeyCode cook;
    [SerializeField] private KeyCode takeClientOrder;
    [SerializeField] private KeyCode cleanDirtyTable;
    [SerializeField] private KeyCode administration;
    [SerializeField] private KeyCode jump;
    [SerializeField] private KeyCode inventory;
    [SerializeField] private KeyCode pause;

    [Header("Sensitivity:")]
    [SerializeField] private float sensitivityX;
    [SerializeField] private float sensitivityY;

    public KeyCode Run { get => run; }
    public KeyCode GrabFood { get => grabFood; }
    public KeyCode HandOverFood { get => handOverFood; }
    public KeyCode Cook { get => cook; }
    public KeyCode TakeClientOrder { get => takeClientOrder; }
    public KeyCode CleanDirtyTable { get => cleanDirtyTable; }
    public KeyCode Administration { get => administration; }
    public KeyCode Jump { get => jump; }
    public KeyCode Inventory { get => inventory; }
    public KeyCode Pause { get => pause; }

    public float SensitivityX { get => sensitivityX; }
    public float SensitivityY { get => sensitivityY; }
}
