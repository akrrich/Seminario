using UnityEngine;

[CreateAssetMenu(fileName = "InputsData", menuName = "ScriptableObjects/Create New InputsData")]
public class InputsData : ScriptableObject
{
    [Header("Tabern Inputs:")]
    [SerializeField] private KeyCode showOrHideDish;

    [Header("Dungeon Inputs:")]
    [SerializeField] private KeyCode dash;
    [SerializeField] private KeyCode attack;

    [Header("Both Inputs:")]
    [SerializeField] private KeyCode interact;
    [SerializeField] private KeyCode run;
    [SerializeField] private KeyCode jump;
    [SerializeField] private KeyCode inventory;
    [SerializeField] private KeyCode pause;

    [Header("Sensitivity:")]
    [SerializeField] private float sensitivityX;
    [SerializeField] private float sensitivityY;

    public KeyCode ShowOrHideDish { get => showOrHideDish; }

    public KeyCode Dash => dash;
    public KeyCode Attack { get => attack; }

    public KeyCode Run { get => run; }
    public KeyCode Jump { get => jump; }
    public KeyCode Interact { get => interact; }
    public KeyCode Inventory { get => inventory; }
    public KeyCode Pause { get => pause; }


    public float SensitivityX { get => sensitivityX; }
    public float SensitivityY { get => sensitivityY; }
}
