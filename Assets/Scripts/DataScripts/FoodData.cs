using UnityEngine;

[CreateAssetMenu(fileName = "FoodData", menuName = "ScriptableObjects/Create New FoodData")]
public class FoodData : ScriptableObject
{
    [SerializeField] private float timeToBeenCooked;

    public float TimeToBeenCooked { get =>  timeToBeenCooked; }
}
