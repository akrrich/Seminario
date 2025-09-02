using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ClientsFoodPreferencesData", menuName = "ScriptableObjects/Tabern/Create New ClientsFoodPreferencesData")]
public class ClientsFoodPreferencesData : ScriptableObject
{
    [SerializeField] private List<FoodChances> foodChances;


    [Serializable]
    public class FoodChances
    {
        [SerializeField] private FoodType foodType;
        [Range(0, 100)]
        [SerializeField] private int probability;

        public FoodType FoodType { get => foodType; }
        public int Probability { get => probability; }
    }


    public FoodType GetRandomFood()
    {
        int roll = UnityEngine.Random.Range(0, 100);
        int cumulative = 0;

        foreach (var option in foodChances)
        {
            cumulative += option.Probability;
            if (roll < cumulative)
                return option.FoodType;
        }

        // En caso de que la suma no llegue a 100, devuelve el ultimo
        return foodChances[foodChances.Count - 1].FoodType;
    }
}
