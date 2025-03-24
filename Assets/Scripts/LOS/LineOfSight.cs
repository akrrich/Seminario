using UnityEngine;

public class LineOfSight
{
    public static bool CheckRange(Transform self, Transform target, float range)
    {
        //A->B
        //B-A
        //A: Self
        //B: Target
        //return Vector3.Distance(self.position, target.position) <= range;

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
}
