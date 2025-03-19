using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerCamera playerCamaera;
    private Rigidbody rb;

    private float speed = 75f;


    void Awake()
    {
        GetComponents();
    }

    void FixedUpdate()
    {
        Movement();
    }


    private void GetComponents()
    {
        rb = GetComponent<Rigidbody>();
        playerCamaera = GetComponentInChildren<PlayerCamera>();
    }

    private void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 cameraForward = playerCamaera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 right = playerCamaera.transform.right;
        Vector3 movement = (cameraForward * verticalInput + right * horizontalInput).normalized * speed * Time.deltaTime;

        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }
}
