using UnityEngine;

[CreateAssetMenu(fileName = "PlayerTabernData", menuName = "ScriptableObjects/Tabern/Create New PlayerTabernData")]
public class PlayerTabernData : ScriptableObject
{
    [Header("LineOfSight:")]
    [SerializeField] private float rangeVision;
    [SerializeField] private float angleVision;

    [Header("Variables:")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float distanceToGrabFood;
    [Header("Jump Check")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float distanceToGround;
    [SerializeField] private LayerMask groundLayer;

    public float RangeVision { get => rangeVision; }
    public float AngleVision { get => angleVision; }

    public float WalkSpeed { get => walkSpeed; }
    public float RunSpeed { get => runSpeed; }
    public float JumpForce { get => jumpForce; }
    public float DistanceToGrabFood { get => distanceToGrabFood; }
    public float DistanceToGround { get => distanceToGround; }
    public LayerMask GroundLayer { get => groundLayer; }

}
