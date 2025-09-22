using UnityEngine;

public class StaminaHandler : MonoBehaviour
{
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float regenRate = 10f;

    public float CurrentStamina { get; private set; }

    private void Start()
    {
        CurrentStamina = maxStamina;
    }

    private void Update()
    {
        if (CurrentStamina < maxStamina)
        {
            CurrentStamina += regenRate * Time.deltaTime;
            CurrentStamina = Mathf.Min(CurrentStamina, maxStamina);
        }
    }

    public bool HasStamina(float amount)
    {
        return CurrentStamina >= amount;
    }

    public void UseStamina(float amount)
    {
        CurrentStamina = Mathf.Max(0, CurrentStamina - amount);
    }
}
