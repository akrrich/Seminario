using UnityEngine;

public class LineOfSight
{
    public static bool CheckRange(Transform self, Transform target, float range)
    {
        Vector3 dir = target.position - self.position;
        float distance = dir.magnitude;
        return distance <= range;
    }

    public static bool CheckAngle(Transform self, Transform target, float angle)
    {
        return CheckAngle(self, target, self.forward, angle);
    }

    public static bool CheckAngle(Transform self, Transform target, Vector3 front, float angle)
    {
        Vector3 dir = target.position - self.position;
        float angleToTarget = Vector3.Angle(front, dir);
        return angleToTarget <= angle / 2;
    }

    public static bool CheckView(Transform self, Transform target, LayerMask obsMask)
    {
        Vector3 dir = target.position - self.position;
        return !Physics.Raycast(self.position, dir.normalized, dir.magnitude, obsMask);
    }

    public static bool LOS(Transform self, Transform target, float range, float angle, LayerMask obsMask)
    {
        return CheckRange(self, target, range) && CheckAngle(self, target, angle) && CheckView(self, target, obsMask);
    }

    public static void DrawLOSOnGizmos(Transform self, float angleVision, float rangeVision)
    {
        int resolutionParable = 30;

        Vector3 origin = self.position;
        float halfAngle = angleVision / 2f;
        float stepAngle = angleVision / resolutionParable;

        Gizmos.color = Color.cyan;

        Vector3 prevPoint = origin + (Quaternion.Euler(0, -halfAngle, 0) * self.forward) * rangeVision;

        for (int i = 1; i <= resolutionParable; i++)
        {
            float currentAngle = -halfAngle + stepAngle * i;
            Vector3 nextPoint = origin + (Quaternion.Euler(0, currentAngle, 0) * self.forward) * rangeVision;

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        Vector3 leftRayDirection = Quaternion.Euler(0, -halfAngle, 0) * self.forward;
        Vector3 rightRayDirection = Quaternion.Euler(0, halfAngle, 0) * self.forward;

        Gizmos.DrawRay(origin, leftRayDirection * rangeVision);
        Gizmos.DrawRay(origin, rightRayDirection * rangeVision);
    }

    public static void GenerateLOSVisual(MeshFilter meshFilter, Transform self, float angleVision, float rangeVision, LayerMask obsMask)
    {
        int resolution = 30;
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[resolution + 2];
        int[] triangles = new int[resolution * 3];

        vertices[0] = Vector3.zero;

        float halfAngle = angleVision / 2f;

        for (int i = 0; i <= resolution; i++)
        {
            float angle = -halfAngle + (angleVision * i / resolution);
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 localDirection = rotation * Vector3.forward;
            Vector3 worldDirection = self.TransformDirection(localDirection);

            Ray ray = new Ray(self.position, worldDirection);
            if (Physics.Raycast(ray, out RaycastHit hit, rangeVision, obsMask))
            {
                vertices[i + 1] = self.InverseTransformPoint(hit.point);
            }
            else
            {
                vertices[i + 1] = localDirection * rangeVision;
            }
        }

        for (int i = 0; i < resolution; i++)
        {
            int baseIndex = i * 3;
            triangles[baseIndex] = 0;
            triangles[baseIndex + 1] = i + 1;
            triangles[baseIndex + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        meshFilter.transform.position = self.position;
        meshFilter.transform.rotation = Quaternion.LookRotation(self.forward, Vector3.up);
    }
}
