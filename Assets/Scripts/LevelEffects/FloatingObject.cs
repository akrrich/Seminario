using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [Header("Floating Configuration")]
    [SerializeField] private float amplitude = 0.5f;        
    [SerializeField] private float frequency = 1f;          
    [SerializeField] private bool useLocalPosition = true;  
    private Vector3 offset = Vector3.zero; 
    
    [Header("Advanced Options")]
    [SerializeField] private bool randomizeStart = false;   
    [SerializeField] private bool smoothMovement = true;    
    
    private Vector3 startPosition;
    private float timeOffset;
    
    void Start()
    {
        startPosition = useLocalPosition ? transform.localPosition : transform.position;
        
        
        if (useLocalPosition)
            transform.localPosition = startPosition + offset;
        else
            transform.position = startPosition + offset;
        
        
        if (randomizeStart)
        {
            timeOffset = Random.Range(0f, 2f * Mathf.PI);
        }
    }
    
    void Update()
    {
        float time = Time.time + timeOffset;
        Vector3 newPosition = startPosition;
        
        if (smoothMovement)
        {
            
            float yOffset = amplitude * Mathf.Sin(time * frequency);
            newPosition += new Vector3(0, yOffset, 0);
        }
        else
        {
            
            float yOffset = amplitude * Mathf.PingPong(time * frequency, 1f);
            newPosition += new Vector3(0, yOffset, 0);
        }
        
        
        if (useLocalPosition)
            transform.localPosition = newPosition;
        else
            transform.position = newPosition;
    }
    
    
    public void SetAmplitude(float newAmplitude)
    {
        amplitude = newAmplitude;
    }
    
    public void SetFrequency(float newFrequency)
    {
        frequency = newFrequency;
    }
    
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
        startPosition = useLocalPosition ? transform.localPosition : transform.position;
        startPosition -= offset;
    }
    
    public void PauseFloating()
    {
        enabled = false;
    }
    
    public void ResumeFloating()
    {
        enabled = true;
    }
}
