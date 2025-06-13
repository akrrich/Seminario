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
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashBufferTime = 0.1f;
    [SerializeField] private bool allowAirDash = false;

    private bool isDashing;
    private Vector3 dashDirection;
    private Transform orientation;

    private void Awake()
    {
        model = GetComponent<PlayerDungeonModel>();
        view = GetComponent<PlayerDungeonView>();
        rb = GetComponent<Rigidbody>();
        orientation = model.transform.Find("Orientation");
    }
    public void ExecuteDash()
    {
        if (isDashing || !model.CanDash) return;

        Vector2 input = PlayerInputs.Instance.GetMoveAxis();

        Vector3 inputDir = (orientation.forward * input.y + orientation.right * input.x).normalized;

        // Normal horizontal dash basado en orientación
        dashDirection = inputDir.sqrMagnitude > 0.01f ? inputDir : orientation.forward;


        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        model.CanMove = false;
        model.SetInvulnerable(true);
        model.RegisterDash();

        view?.PlayDashAnimation();

        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            rb.velocity = dashDirection * dashSpeed;
            yield return null;
        }

        // Opcional: seguir moviéndote por inercia o parar en seco
        rb.velocity = Vector3.zero;

        model.CanMove = true;
        model.SetInvulnerable(false);

        isDashing = false;
    }
}
