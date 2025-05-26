using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    [Header("General Settings")]
    [Min(1)]
    [SerializeField] private int maxObs; 
    [Range(1f, 20f)] 
    [SerializeField] private float radius;

    [Range(30f, 180f)] 
    [SerializeField] private float angle;

    [Header("Personal Area and Layers")]
    [SerializeField] private float personalArea = 1.5f; 
    [SerializeField] private LayerMask obsMask; 

    private Collider[] colls;

    public Vector3 Self => transform.position;


    void Awake()
    {
        InitializeColliders();
        //personalAreaGetComponent<Collider>().bounds.extents.magnitude;
    }

    void OnDrawGizmosSelected()
    {
        DrawOnGizmosArea();
    }


    public Vector3 GetDir(Vector3 currDir)
    {
        int count = Physics.OverlapSphereNonAlloc(Self, radius, colls, obsMask);

        Collider nearColl = null;
        float nearCollDistance = 0;
        Vector3 nearClosestPoint = Vector3.zero;

        for (int i = 0; i < count; i++)
        {
            Collider currColl = colls[i];
            Vector3 closestPoint = currColl.ClosestPoint(Self);
            Vector3 dir = closestPoint - Self;
            float distance = dir.magnitude;

            var currAngle = Vector3.Angle(dir, currDir);
            if (currAngle > angle / 2) continue;

            if (nearColl == null || distance < nearCollDistance)
            {
                nearColl = currColl;
                nearCollDistance = distance;
                nearClosestPoint = closestPoint;
            }
        }

        if (nearColl == null)
        {
            return currDir; // No obstáculo, sigue normal
        }

        Vector3 relativePos = transform.InverseTransformPoint(nearClosestPoint);
        Vector3 dirToColl = (nearClosestPoint - Self).normalized;
        Vector3 avoidanceDir = Vector3.Cross(transform.up, dirToColl);

        if (relativePos.x > 0)
        {
            avoidanceDir = -avoidanceDir;
        }

        Debug.DrawRay(Self, avoidanceDir * 2, Color.red);

        Vector3 finalDir = Vector3.Lerp(currDir, avoidanceDir, (radius - Mathf.Clamp(nearCollDistance - personalArea, 0, radius)) / radius);

        // Nueva mejora: Si después del avoidance el movimiento es muy pequeño, intentamos buscar otra dirección
        if (finalDir.magnitude < 0.1f)
        {
            finalDir = TryFindAlternateDirection(currDir);
        }

        return finalDir.normalized;
    }


    private Vector3 TryFindAlternateDirection(Vector3 currDir)
    {
        float[] testAngles = { 30f, -30f, 60f, -60f, 90f, -90f,120f,-120f }; // Angulos para intentar girar

        foreach (float angle in testAngles)
        {
            Vector3 testDir = Quaternion.Euler(0, angle, 0) * currDir;

            if (!Physics.Raycast(Self, testDir, personalArea * 2f, obsMask)) // Si el rayo no choca, es camino libre
            {
                Debug.DrawRay(Self, testDir * 2, Color.magenta); // Para debug visual
                return testDir;
            }
        }

        // Si no encuentra salida, sigue en la dirección original
        return currDir;
    }

    private void InitializeColliders()
    {
        colls = new Collider[maxObs];
    }

    private void DrawOnGizmosArea()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius); // Draw the avoidance radius

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, personalArea); // Draw the personal area

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, angle / 2, 0) * transform.forward * radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -angle / 2, 0) * transform.forward * radius);
    }
}
