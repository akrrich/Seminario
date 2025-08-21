using UnityEngine;

public class ClericEnemyModel : HumanEnemyModel
{
    private float lastHealTime;
    private const float healCooldown = 10f;
    private const int healAmount = 30;

    protected override void Update()
    {
        base.Update();
        if (CanHeal())
        {
            Heal();
        }
    }

    private bool CanHeal()
    {
        return Time.time - lastHealTime > healCooldown && CurrentHP <= MaxHP * 0.5f;
    }

    private void Heal()
    {
        currentHP = Mathf.Min(MaxHP, currentHP + healAmount);
        lastHealTime = Time.time;
        Debug.Log("Clérigo se curó.");
    }

    protected override void DropLoot()
    {
        float roll = Random.value;
        //if (roll < 0.4f) LootManager.Instance.Spawn("CarneHumana", transform.position);
        //else if (roll < 0.6f) LootManager.Instance.Spawn("VinoUvaOscura", transform.position);
        //else if (roll < 0.7f) CurrencyManager.Instance.AddDientes(30);
    }
}
