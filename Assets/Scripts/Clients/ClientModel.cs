using UnityEngine;

public enum ClientStates
{
    idle, GoChair, Leave
}

public class ClientModel : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] public Transform newTransform;
    [SerializeField] public Transform startTransform;

    [SerializeField] private float speed;


    void Awake()
    {
        GetComponents();
    }


    private void GetComponents()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void MoveToTarget(Transform target)
    {
        //transform.LookAt(target);

        Vector3 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * speed * Time.fixedDeltaTime;
    }

    public void StopVelocity()
    {
        rb.velocity = Vector3.zero;
    }
}
