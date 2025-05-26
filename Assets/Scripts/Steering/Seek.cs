using UnityEngine;

public class Seek : ISteering
{
    private Transform self;
    private Transform target;

    public Seek(Transform self, Transform target)
    {
        this.self = self;
        this.target = target;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public virtual Vector3 GetDir()
    {
       return (target.position - self.position).normalized;
    }
}
