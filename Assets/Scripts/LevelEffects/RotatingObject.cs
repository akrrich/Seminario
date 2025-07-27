using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    [Header("Rotation Configuration")]
    [SerializeField] private bool rotateX = false;
    [SerializeField] private bool rotateZ = false;
    [SerializeField] private float rotationSpeedX = 50f;
    [SerializeField] private float rotationSpeedZ = 50f;
    
    [Header("Random Rotation")]
    [SerializeField] private bool enableRandomRotation = false;
    [SerializeField] private float randomRotationSpeed = 30f;
    [SerializeField] private float randomRotationChangeInterval = 2f;
    
    [Header("Random Direction Options")]
    [SerializeField] private bool randomX = true;
    [SerializeField] private bool randomY = false;
    [SerializeField] private bool randomZ = true;
    
    private Vector3 randomRotationDirection;
    private float lastRandomChangeTime;
    
    void Start()
    {
        if (enableRandomRotation)
        {
            GenerateNewRandomDirection();
        }
    }
    
    void Update()
    {
        
        if (rotateX)
        {
            transform.Rotate(Vector3.right, rotationSpeedX * Time.deltaTime);
        }
        
        if (rotateZ)
        {
            transform.Rotate(Vector3.forward, rotationSpeedZ * Time.deltaTime);
        }
        
        
        if (enableRandomRotation)
        {
            
            if (Time.time - lastRandomChangeTime >= randomRotationChangeInterval)
            {
                GenerateNewRandomDirection();
                lastRandomChangeTime = Time.time;
            }
            
            
            transform.Rotate(randomRotationDirection, randomRotationSpeed * Time.deltaTime);
        }
    }
    
    private void GenerateNewRandomDirection()
    {
        randomRotationDirection = Vector3.zero;
        
        if (randomX)
            randomRotationDirection.x = Random.Range(-1f, 1f);
        if (randomY)
            randomRotationDirection.y = Random.Range(-1f, 1f);
        if (randomZ)
            randomRotationDirection.z = Random.Range(-1f, 1f);
            
        
        if (randomRotationDirection != Vector3.zero)
        {
            randomRotationDirection.Normalize();
        }
    }
    
    
    public void SetRotationX(bool enabled, float speed = 50f)
    {
        rotateX = enabled;
        rotationSpeedX = speed;
    }
    
    public void SetRotationZ(bool enabled, float speed = 50f)
    {
        rotateZ = enabled;
        rotationSpeedZ = speed;
    }
    
    public void SetRandomRotation(bool enabled, float speed = 30f)
    {
        enableRandomRotation = enabled;
        randomRotationSpeed = speed;
        
        if (enabled)
        {
            GenerateNewRandomDirection();
        }
    }
    
    public void SetRandomDirections(bool x, bool y, bool z)
    {
        randomX = x;
        randomY = y;
        randomZ = z;
        
        if (enableRandomRotation)
        {
            GenerateNewRandomDirection();
        }
    }
}
