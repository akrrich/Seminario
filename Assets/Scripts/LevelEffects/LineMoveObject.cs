using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMoveObject : MonoBehaviour
{
    [Header("Start Point (A)")]
    public Transform pointA;
    [Header("End Point (B)")]
    public Transform pointB;
    [Header("Movement speed ")]
    public float speed = 1f;

    private Vector3 currentTarget;

    void Start()
    {
        if (pointA != null)
            transform.position = pointA.position;
        currentTarget = pointB != null ? pointB.position : transform.position;
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, currentTarget) < 0.01f)
        {
            currentTarget = currentTarget == pointA.position ? pointB.position : pointA.position;
        }
    }

    void OnDrawGizmos()
    {
        if (pointA != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pointA.position, 0.1f);
        }
        if (pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pointB.position, 0.1f);
        }
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
