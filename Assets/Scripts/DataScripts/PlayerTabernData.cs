using UnityEngine;

[CreateAssetMenu(fileName = "PlayerTabernData", menuName = "ScriptableObjects/Create New PlayerTabernData")]
public class PlayerTabernData : ScriptableObject
{
    [Header("LineOfSight:")]
    [SerializeField] private float rangeVision;
    [SerializeField] private float angleVision;

    [Header("Variables:")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;

    public float RangeVision { get => rangeVision; }
    public float AngleVision { get => angleVision; }

    public float WalkSpeed { get => walkSpeed; }
    public float RunSpeed { get => runSpeed; }
    public float JumpForce { get => jumpForce; }
}
