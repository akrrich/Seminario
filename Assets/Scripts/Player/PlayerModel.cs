using UnityEngine;

public enum PlayerStates
{
    Idle,
    Walk,
    Jump
}

public class PlayerModel : MonoBehaviour
{
    private PlayerCamera playerCamaera;
    private Rigidbody rb;

    [SerializeField] private float speed = 250f;
    private bool isGrounded = true;

    public Rigidbody Rb { get => rb; set => rb = value; }

    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }


    void Awake()
    {
        GetComponents();
    }

    void FixedUpdate()
    {
        Movement();
    }


    public static Vector2 GetMove()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }


    private void GetComponents()
    {
        rb = GetComponent<Rigidbody>();
        playerCamaera = GetComponentInChildren<PlayerCamera>();
    }

    private void Movement()
    {
        Vector3 cameraForward = playerCamaera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 right = playerCamaera.transform.right;
        Vector3 movement = (cameraForward * GetMove().y + right * GetMove().x).normalized * speed * Time.deltaTime;

        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }
}
