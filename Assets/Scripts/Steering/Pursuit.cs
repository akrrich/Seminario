using UnityEngine;

public class Pursuit : ISteering
{
    private Transform self;
    private Rigidbody target;

    private float timePrediction;
    private float errorRange = 0.1f;

    public float TimePrediction { get => timePrediction; set => timePrediction = value; }
    

    public Pursuit(Transform self, Rigidbody target, float errorRange = 0, float timePrediction = 0)
    {
        this.self = self;
        this.target = target;
        this.timePrediction = timePrediction;
    }

    public Pursuit(Transform self, Rigidbody target, float errorRange = 0)
    {
        this.self = self;
        this.target = target;
    }

    public Pursuit(Transform self, Rigidbody target)
    {
        this.self = self;
        this.target = target;
    }


    public virtual Vector3 GetDir()
    {
        Vector3 point = target.position + target.velocity * timePrediction;
        Vector3 dirToPoint = (point - self.position).normalized;
        Vector3 dirToTarget = (target.position - self.position).normalized;

        if (Vector3.Dot(dirToPoint, dirToTarget) < 0 + errorRange)
        {
            return dirToTarget;
        }

        else
        {
            return dirToPoint;
        }
    }
}
