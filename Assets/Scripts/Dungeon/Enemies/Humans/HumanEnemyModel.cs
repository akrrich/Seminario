using UnityEngine;

public class HumanEnemyModel : BaseEnemyModel
{
    [Header("Lógica Humana")]
    [SerializeField] private int mana;
    [SerializeField] private int maxMana = 50;
    [SerializeField] private float manaRegenRate = 2f;

    public int Mana => mana;
    public int MaxMana => maxMana;

    protected override void Update()
    {
        RegenerateMana();
    }

    private void RegenerateMana()
    {
        if (mana < maxMana)
        {
            mana += Mathf.CeilToInt(manaRegenRate * Time.deltaTime);
            mana = Mathf.Min(mana, maxMana);
        }
    }

    public bool HasEnoughMana(int cost)
    {
        return mana >= cost;
    }

    public void ConsumeMana(int cost)
    {
        mana = Mathf.Max(0, mana - cost);
    }
}
