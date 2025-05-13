using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    private static PlayerInputs instance;

    [Header("Keyboard Inputs:")]
    [SerializeField] private KeyCode run;
    [SerializeField] private KeyCode grabFood;
    [SerializeField] private KeyCode handOverFood;
    [SerializeField] private KeyCode cook;
    [SerializeField] private KeyCode administration;
    [SerializeField] private KeyCode jump;
    [SerializeField] private KeyCode inventory;
    [SerializeField] private KeyCode pause;

    [Header("Joystick Inputs:")]

    public static PlayerInputs Instance { get => instance; }


    void Awake()
    {
        CreateSingleton();
    }


    public Vector2 GetMoveAxis()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public bool Run()
    {
        return Input.GetKey(run);
    }

    public bool StopRun()
    {
        return Input.GetKeyUp(run);
    }

    public bool GrabFood()
    {
        return Input.GetKeyDown(grabFood);
    }

    public bool HandOverFood()
    {
        return Input.GetKeyDown(handOverFood);
    }

    public bool Jump()
    {
        return Input.GetKeyDown(jump);
    }

    public bool Cook()
    {
        return Input.GetKeyDown(cook);
    }

    public bool Administration()
    {
        return Input.GetKeyDown(administration);
    }

    public bool Inventory()
    {
        return Input.GetKeyDown(inventory);
    }

    public bool Pause()
    {
        return Input.GetKeyDown(pause);
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
        }
    }
}
