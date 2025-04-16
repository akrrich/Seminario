using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    private static PlayerInputs instance;

    [Header("Keyboard Inputs:")]
    [SerializeField] private KeyCode run;
    [SerializeField] private KeyCode grabItem;
    [SerializeField] private KeyCode dropItem;
    [SerializeField] private KeyCode grabFood;
    [SerializeField] private KeyCode handOverFood;
    [SerializeField] private KeyCode cook;
    [SerializeField] private KeyCode administration;
    [SerializeField] private KeyCode jump;

    public static PlayerInputs Instance { get => instance; }


    void Awake()
    {
        CreateSingleton();
    }


    public bool Run()
    {
        return Input.GetKey(run);
    }

    public bool StopRun()
    {
        return Input.GetKeyUp(run);
    }

    public bool GrabItem()
    {
        return Input.GetKeyDown(grabItem);    
    }

    public bool DropItem()
    {
        return Input.GetKeyDown(dropItem);
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
