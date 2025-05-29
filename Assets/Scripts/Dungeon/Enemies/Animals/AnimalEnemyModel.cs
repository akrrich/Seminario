using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalEnemyModel : BaseEnemyModel
{
    [Header("Animal Behavior Settings")]
    [SerializeField] private float idleRoamRadius = 5f;
    [SerializeField] private float idleRoamInterval = 2f;

    private float roamTimer;

    public float RoamRadius => idleRoamRadius;

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Movimiento aleatorio en idle, para todos los animales
    /// </summary>
    public void IdleRoam()
    {
        roamTimer += Time.deltaTime;
        if (roamTimer >= idleRoamInterval)
        {
            roamTimer = 0f;
            Vector3 randomOffset = Random.insideUnitSphere * idleRoamRadius;
            randomOffset.y = 0f;
            Vector3 target = transform.position + randomOffset;
            MoveTo(target);
        }
    }

    public virtual void OnPlayerSpotted()
    {
        // Animales pueden emitir un sonido o alertar otros
        Debug.Log($"{gameObject.name} spotted player!");
    }

}
