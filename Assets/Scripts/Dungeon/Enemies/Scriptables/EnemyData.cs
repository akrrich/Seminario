using UnityEngine;
[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public int HP;
    public int Damage;
    public float Speed;
    public float AttackCooldown;
}
