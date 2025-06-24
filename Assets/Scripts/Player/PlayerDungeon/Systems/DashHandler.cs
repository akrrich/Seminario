using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashHandler : MonoBehaviour
{
    private PlayerDungeonModel model;
    private PlayerDungeonView view;
    private Rigidbody rb;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashBufferTime = 0.1f;
    [SerializeField] private bool allowAirDash = false;

    private bool isDashing;
    private float bufferTimer;   
    private bool dashQueued;    
    private Vector3 dashDirection;
    private Transform orientation;

    #region Unity Lifecycle
    private void Awake()
    {
        model = GetComponent<PlayerDungeonModel>();
        view = GetComponent<PlayerDungeonView>();
        rb = GetComponent<Rigidbody>();
        orientation = model.transform.Find("Orientation");
    }

    private void Update()
    {
        BufferTick();      // gestiona el dash en cola (nuevo)
    }
    #endregion
    public void ExecuteDashRequest()
    {
        // 1-) ¿Se permite dash en el aire?
        if (!allowAirDash && !model.IsGrounded) return;

        // 2-) Si ya está dasheando, o el cooldown no terminó, lo coloco en buffer
        if (isDashing || !model.CanDash)
        {
            dashQueued = true;
            bufferTimer = dashBufferTime;
            return;
        }

        StartDash();
    }
    private void BufferTick()
    {
        if (!dashQueued) return;

        // Cuenta regresiva
        bufferTimer -= Time.deltaTime;

        // Si el buffer expiró, descarto la petición
        if (bufferTimer <= 0f)
        {
            dashQueued = false;
            return;
        }

        // Si ya puedo dashear, ejecuto y limpio la cola
        if (model.CanDash && !isDashing)
        {
            dashQueued = false;
            StartDash();
        }
    }
    private void StartDash()
    {
        // Dirección basada en input actual
        Vector2 input = PlayerInputs.Instance.GetMoveAxis();
        Vector3 inputDir = (orientation.forward * input.y + orientation.right * input.x).normalized;
        dashDirection = inputDir.sqrMagnitude > 0.01f ? inputDir : orientation.forward;

        StartCoroutine(DashRoutine());
    }
    private IEnumerator DashRoutine()
    {
        isDashing = true;
        model.CanMove = false;
        model.SetInvulnerable(true);
        model.RegisterDash();         // inicia cooldown
        view?.PlayDashAnimation();

        float start = Time.time;
        while (Time.time < start + dashDuration)
        {
            rb.velocity = dashDirection * dashSpeed;
            yield return null;
        }

        rb.velocity = Vector3.zero;
        model.CanMove = true;
        model.SetInvulnerable(false);
        isDashing = false;
    }
}
