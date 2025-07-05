using UnityEngine;
using System;
using System.Collections;

public enum PlayerStates
{
    Idle, Walk, Run, Jump, Cook, Admin
}

public class PlayerModel : MonoBehaviour
{
    [SerializeField] private PlayerTabernData playerTabernData;

    private PlayerCamera playerCamera;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private PhysicMaterial physicMaterial;

    private GameObject cookingDeskUI;
    private GameObject oven;
    private GameObject administration;

    private static event Action<PlayerModel> onPlayerInitialized; // Evento que se usa para buscar referencias al player desde escenas aditivas

    private float speed;

    private bool isGrounded = true;
    private bool isCollidingCookingDeskUI = false;
    private bool isCollidingAdministration = false;
    private bool isCooking = false;
    private bool isAdministrating = false;

    public PlayerTabernData PlayerTabernData { get => playerTabernData; }

    public PlayerCamera PlayerCamera { get => playerCamera; set => playerCamera = value; }

    public Rigidbody Rb { get => rb; }
    public CapsuleCollider CapsuleCollider { get => capsuleCollider; set => capsuleCollider = value; }
    public PhysicMaterial PhysicsMaterial { get => physicMaterial; }
    public GameObject CookingDeskUI { get => cookingDeskUI; }
    public GameObject Administration { get => administration; }

    public static Action<PlayerModel> OnPlayerInitialized { get => onPlayerInitialized; set => onPlayerInitialized = value; }

    public float Speed { get => speed; set => speed = value; }
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
    public bool IsCollidingCookingDeskUI { get => isCollidingCookingDeskUI; set => isCollidingCookingDeskUI = value; }
    public bool IsCollidingAdministration { get => isCollidingAdministration; set => isCollidingAdministration = value; }   
    public bool IsCooking { get => isCooking; set => isCooking = value; }
    public bool IsAdministrating { get => isAdministrating; set => isAdministrating = value; }


    void Awake()
    {
        GetComponents();
        Initialize();

        // Provisorio
        StartCoroutine(InvokeEventInitializationPlayer());
    }

    void FixedUpdate()
    {
        Movement();
    }

    void OnDrawGizmosSelected()
    {
        LineOfSight.DrawLOSOnGizmos(transform, playerTabernData.AngleVision, playerTabernData.RangeVision);
    }


    /// <summary>
    /// Solucionar los LineOfSight de que mirando hacia un poco fuera del rango funcionan
    /// </summary>
    public bool IsLookingAtCookingDeskUI()
    {
        return LineOfSight.LOS(playerCamera.transform, cookingDeskUI.transform, playerTabernData.RangeVision, playerTabernData.AngleVision, LayerMask.GetMask("Obstacles"));
    }

    public bool IsLookingAtAdministration()
    {
        return LineOfSight.LOS(playerCamera.transform, administration.transform, playerTabernData.RangeVision, playerTabernData.AngleVision, LayerMask.GetMask("Obstacles"));
    }

    // Ajustar el rayo correctamente
    public Food IsLookingAtFood()
    {
        if (Vector3.Distance(transform.position, oven.transform.position) <= 5f)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward + Vector3.up * 0.05f);
            float rayDistance = 5f;

            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red, 0.2f);

            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            {
                if (hit.collider.CompareTag("FoodTrigger"))
                {
                    Debug.Log("Yes");
                    return hit.collider.GetComponentInParent<Food>();
                }
            }
        }

        return null;
    }

    public void LookAt(Vector3 target)
    {
        Vector3 newDirection = (target - transform.position).normalized;
        Vector3 lookDirection = new Vector3(newDirection.x, 0, newDirection.z);

        if (lookDirection != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = rotation;
        }
    }


    private void GetComponents()
    {
        playerCamera = GetComponentInChildren<PlayerCamera>();
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        cookingDeskUI = GameObject.FindGameObjectWithTag("CookingDeskUI");
        oven = GameObject.FindGameObjectWithTag("Oven");
        administration = GameObject.FindGameObjectWithTag("Administration");
    }

    private void Initialize()
    {
        physicMaterial = capsuleCollider.material;
    }

    private void Movement()
    {
        if (PlayerInputs.Instance != null && !isCooking && !isAdministrating)
        {
            Vector3 cameraForward = playerCamera.transform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            Vector3 right = playerCamera.transform.right;
            Vector3 movement = (cameraForward * PlayerInputs.Instance.GetMoveAxis().y + right * PlayerInputs.Instance.GetMoveAxis().x).normalized * speed * Time.fixedDeltaTime;

            rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
        }
    }

    private IEnumerator InvokeEventInitializationPlayer()
    {
        yield return new WaitForSeconds(1);

        onPlayerInitialized?.Invoke(this);
    }
}
