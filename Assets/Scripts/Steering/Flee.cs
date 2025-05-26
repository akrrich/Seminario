using UnityEngine;

public class Flee : Seek
{
    public Flee(Transform self, Transform target) : base(self, target)
    {
    }

    public override Vector3 GetDir()
    {
        return -base.GetDir(); 
    }
}
