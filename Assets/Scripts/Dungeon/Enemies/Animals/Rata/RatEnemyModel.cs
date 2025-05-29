using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class RatEnemyModel : AnimalEnemyModel
{
    protected override void Awake()
    {
        base.Awake();
        Agent.angularSpeed = 720f; // para giros erráticos
        Agent.acceleration = 100f;
    }

    protected override void DropLoot()
    {
        float roll = Random.Range(0f, 1f);
        if (roll < 0.1f)
            LootManager.Instance.SpawnLoot("CarneBestia", transform.position);
        else if (roll < 0.3f)
            MoneyManager.Instance.AddMoney(10);
    }

    // Movimiento errático: llamado desde estado Chase
    public Vector3 GetRandomCircleTargetAroundPlayer()
    {
        Vector3 center = Player.transform.position;
        float radius = 4f;
        Vector2 randCircle = Random.insideUnitCircle.normalized * radius;
        return center + new Vector3(randCircle.x, 0f, randCircle.y);
    }
}
