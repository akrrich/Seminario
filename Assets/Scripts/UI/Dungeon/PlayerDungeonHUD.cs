using System;


public class PlayerDungeonHUD
{
    public static Action<float, float> OnHealthChanged; // currentHP, maxHP
    public static Action OnPlayerDeath;

    public static Action<float, float> OnStaminaChanged;
}
