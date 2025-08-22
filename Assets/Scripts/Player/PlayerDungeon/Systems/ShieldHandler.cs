using System.Collections;
using UnityEngine;

public class ShieldHandler : MonoBehaviour
{
    private PlayerDungeonModel model;
    private PlayerDungeonView view;

    [SerializeField] private float staminaCost = 25f;
    [SerializeField] private float cooldown = 1.5f;
    [SerializeField] private float activeTime = 0.6f; // cu�nto dura levantado el escudo

    private bool isActive;
    private float lastUseTime = -999f;

    private void Awake()
    {
        model = GetComponent<PlayerDungeonModel>();
        view = GetComponent<PlayerDungeonView>();
    }

    public void TryUseShield()
    {
        if (isActive) return;
        if (Time.time < lastUseTime + cooldown) return;
        if (model.CurrentStamina < staminaCost) return;

        // Gasto de stamina
        model.UseStamina(staminaCost);

        // Activaci�n
        isActive = true;
        lastUseTime = Time.time;
        //view?.PlayShieldAnimation(); // animaci�n de levantar escudo
        StartCoroutine(CO_ShieldDuration());
    }

    private IEnumerator CO_ShieldDuration()
    {
        yield return new WaitForSeconds(activeTime);
        isActive = false;
       // view?.StopShieldAnimation();
    }

    public bool IsActive => isActive;
}
