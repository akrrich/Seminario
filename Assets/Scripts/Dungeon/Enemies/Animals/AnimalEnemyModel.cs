using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalEnemyModel : BaseEnemyModel
{
    [Header("Animal Behavior Settings")]
    [SerializeField] private float idleRoamRadius = 5f;
    //[SerializeField] private float idleRoamInterval = 2f;

    private float roamTimer;

    public float RoamRadius => idleRoamRadius;

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Movimiento aleatorio en idle, para todos los animales
    /// </summary>
 

    public virtual void OnPlayerSpotted()
    {
        // Animales pueden emitir un sonido o alertar otros
        Debug.Log($"{gameObject.name} spotted player!");
    }

}
