using UnityEngine;

public class RatEnemyController : AnimalEnemyController
{
    private RatEnemyModel ratModel;

    protected override void GetComponents()
    {
        base.GetComponents();
        ratModel = GetComponent<RatEnemyModel>();
    }

   
}