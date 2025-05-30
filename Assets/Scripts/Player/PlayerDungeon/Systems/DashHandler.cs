using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashHandler : MonoBehaviour
{
    private PlayerDungeonModel model;
    private PlayerDungeonView view;

    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float invulnDuration = 0.3f;

    private void Awake()
    {
        model = GetComponent<PlayerDungeonModel>();
        view = GetComponent<PlayerDungeonView>();
    }

    public void ExecuteDash()
    {
        model.RegisterDash();
        model.SetInvulnerable(true);
        view.PlayDashAnimation();

        Vector3 dashDir = PlayerInputs.Instance.GetMoveAxis().normalized;

        if (dashDir.sqrMagnitude > 0.01f)
        {
            // Move via Rigidbody if available
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(transform.TransformDirection(dashDir) * dashForce, ForceMode.Impulse);
            }
            else
            {
                transform.position += transform.TransformDirection(dashDir) * dashForce;
            }
        }
    }

    public float GetDashDuration() => invulnDuration;
}
