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
    [SerializeField] private float dashTapThreshold = 0.25f;
   
    private bool isDashing;
    private float bufferTimer;   
    private bool dashQueued;    
    private Vector3 dashDirection;
    private Transform orientation;

    private bool runKeyDown = false;
    private float runKeyDownTime = 0f;
    private bool runHeldFlag = false;
    private bool dashThisFrame = false;

    private bool runButtonDown = false;
    private bool runButtonHeld = false;
    private bool runButtonUp = false;
    
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
        HandleRunDashInput();
        BufferTick();
        if (dashThisFrame)
        {
            dashThisFrame = false;
            ExecuteDashRequest();
        }
    }
    #endregion
    public bool IsRunHeld() => runHeldFlag;
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
        if(PlayerInputs.Instance != null)
        {
         Vector2 input = PlayerInputs.Instance.GetMoveAxis();
         Vector3 inputDir = (orientation.forward * input.y + orientation.right * input.x).normalized;
         dashDirection = inputDir.sqrMagnitude > 0.01f ? inputDir : orientation.forward;

         StartCoroutine(DashRoutine());
        }
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
    private void HandleRunDashInput()
    {
        if (PlayerInputs.Instance != null)
        {
            var key = PlayerInputs.Instance.KeyboardInputs.Run;
            var joy = PlayerInputs.Instance.JoystickInputs.Run;

            runButtonDown = Input.GetKeyDown(key) || Input.GetKeyDown(joy);
            runButtonHeld = Input.GetKey(key) || Input.GetKey(joy);
            runButtonUp = Input.GetKeyUp(key) || Input.GetKeyUp(joy);

            if (runButtonDown)
            {
                runKeyDown = true;
                runKeyDownTime = Time.time;
                runHeldFlag = false;
            }

            if (runKeyDown && !runHeldFlag && runButtonHeld &&
                Time.time - runKeyDownTime >= dashTapThreshold)
            {
                runHeldFlag = true;
            }

            if (runButtonUp && runKeyDown)
            {
                if (!runHeldFlag && Time.time - runKeyDownTime < dashTapThreshold)
                    dashThisFrame = true;

                runKeyDown = false;
                runHeldFlag = false;
            }
        }
    }
    
}
