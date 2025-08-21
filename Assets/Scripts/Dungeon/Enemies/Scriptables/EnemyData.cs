using UnityEngine;
[CreateAssetMenu(
    fileName = "EnemyData",
    menuName = "ScriptableObjects/Enemy Data",
    order = 0)]
public class EnemyData : ScriptableObject
{
    [Header("Core Stats")]
    public int HP = 30;   // Vida m�xima
    public int Damage = 5;    // Da�o por mordisco
    public float Speed = 6f;   // Velocidad NavMeshAgent
    [Tooltip("Segundos entre mordiscos")]
    public float AttackCooldown = 1.5f; // Cooldown en segundos

    [Header("Combate")]
    [Tooltip("Distancia m�xima (en metros) para intentar morder")]
    public float DistanceToPlayer = 3f;  // Rango de ataque
}
