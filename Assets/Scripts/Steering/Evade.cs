using UnityEngine;

public class Evade : Pursuit
{
    public Evade(Transform self, Rigidbody target) : base(self, target)
    {

    }

    public Evade(Transform self, Rigidbody target, float errorRange = 0) : base(self, target, errorRange)
    {

    }

    public Evade(Transform self, Rigidbody target, float errorRange = 0, float timePrediction = 0) : base(self, target, errorRange, timePrediction)
    {

    }

    public override Vector3 GetDir()
    {
        return -base.GetDir();
    }
}
