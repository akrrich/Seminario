using UnityEngine;

[CreateAssetMenu(fileName = "InputsData", menuName = "ScriptableObjects/Create New InputsData")]
public class InputsData : ScriptableObject
{
    [Header("Tabern Inputs:")]
    [SerializeField] private KeyCode grabFood;
    [SerializeField] private KeyCode handOverFood;
    [SerializeField] private KeyCode cook;
    [SerializeField] private KeyCode takeClientOrder;
    [SerializeField] private KeyCode cleanDirtyTable;
    [SerializeField] private KeyCode administration;
    [SerializeField] private KeyCode throwFoodToTrash;
    [SerializeField] private KeyCode showOrHideDish;

    [Header("Dungeon Inputs:")]
    [SerializeField] private KeyCode dash;
    [SerializeField] private KeyCode interact;

    [Header("Both Inputs:")]
    [SerializeField] private KeyCode run;
    [SerializeField] private KeyCode jump;
    [SerializeField] private KeyCode inventory;
    [SerializeField] private KeyCode pause;

    [Header("Sensitivity:")]
    [SerializeField] private float sensitivityX;
    [SerializeField] private float sensitivityY;

    public KeyCode GrabFood { get => grabFood; }
    public KeyCode HandOverFood { get => handOverFood; }
    public KeyCode Cook { get => cook; }
    public KeyCode TakeClientOrder { get => takeClientOrder; }
    public KeyCode CleanDirtyTable { get => cleanDirtyTable; }
    public KeyCode Administration { get => administration; }
    public KeyCode ThrowFoodToTrash { get => throwFoodToTrash; }
    public KeyCode ShowOrHideDish { get => showOrHideDish; }

    public KeyCode Run { get => run; }
    public KeyCode Jump { get => jump; }
    public KeyCode Dash => dash;
    public KeyCode Interact => interact;

    public KeyCode Inventory { get => inventory; }
    public KeyCode Pause { get => pause; }

    public float SensitivityX { get => sensitivityX; }
    public float SensitivityY { get => sensitivityY; }
}
