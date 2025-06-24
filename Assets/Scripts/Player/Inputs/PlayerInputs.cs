using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    // ────────────────────── Singleton ──────────────────────
    private static PlayerInputs instance;
    public static PlayerInputs Instance => instance;

    // ────────────────── Asignables en Inspector ─────────────
    [SerializeField] private Inputs keyboardInputs;
    [SerializeField] private Inputs joystickInputs;

    [Header("Dash/Run Settings")]
    [Tooltip("Máximo tiempo (s) que cuenta como TAP para dash")]
    [SerializeField] private float dashTapThreshold = 0.2f;

    // ────────────────── Internos ────────────────────────────
    private PlayerInputActions inputActions;          // Input System asset
    private Vector2 joystick = Vector2.zero;          // rotación analógico

    // Dash / Run state machine
    private bool runKeyDown;
    private float runKeyDownTime;
    private bool runHeldFlag;
    private bool dashThisFrame;

    public Inputs KeyboardInputs { get => keyboardInputs; }
    public Inputs JoystickInputs { get => joystickInputs; }
    void Awake()
    {
        CreateSingleton();
        InitializePlayerInputActions();
    }

    private void Update() => HandleRunDashTap();
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

    public bool Attack() => Input.GetMouseButtonDown(0); //Click izquierdo
    public bool Dash()
    {
        if (dashThisFrame)
        {
            dashThisFrame = false;      // Consumimos
            return true;
        }
        return false;
    }
    public bool RunHeld() => runHeldFlag;
    public bool Interact() => Input.GetKeyDown(keyboardInputs.Interact);

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
    private void HandleRunDashTap()
    {
        bool down = Input.GetKeyDown(keyboardInputs.Run) || Input.GetKeyDown(joystickInputs.Run);
        bool held = Input.GetKey(keyboardInputs.Run) || Input.GetKey(joystickInputs.Run);
        bool up = Input.GetKeyUp(keyboardInputs.Run) || Input.GetKeyUp(joystickInputs.Run);

        // ── Al presionar ──
        if (down)
        {
            runKeyDown = true;
            runKeyDownTime = Time.time;
            runHeldFlag = false;
        }

        // ── Manteniendo ──
        if (runKeyDown && !runHeldFlag && held &&
            Time.time - runKeyDownTime >= dashTapThreshold)
        {
            runHeldFlag = true;            // ahora es carrera
        }

        // ── Al soltar ──
        if (up && runKeyDown)
        {
            if (!runHeldFlag && Time.time - runKeyDownTime < dashTapThreshold)
                dashThisFrame = true;      // fue un TAP ⇒ dash

            runKeyDown = false;
            runHeldFlag = false;
        }
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

    [Header("Dungeon Inputs:")]
    [SerializeField] private KeyCode interact;

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
    public KeyCode Interact => interact;
    public float SensitivityX { get => sensitivityX; }
    public float SensitivityY { get => sensitivityY; }
}
