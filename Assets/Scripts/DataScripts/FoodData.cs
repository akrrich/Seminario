using UnityEngine;

[CreateAssetMenu(fileName = "FoodData", menuName = "ScriptableObjects/Tabern/Create New FoodData")]
public class FoodData : ScriptableObject
{
    [SerializeField] private float timeToBeenCooked;
    [SerializeField] private float timeToBeenBurned;

    public float TimeToBeenCooked { get =>  timeToBeenCooked; }
    public float TimeToBeenBurned { get => timeToBeenBurned; }
}
