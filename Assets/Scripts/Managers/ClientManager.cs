using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition, outsidePosition;

    [SerializeField] private ObjectPooler clientPool;
    [SerializeField] private List<Table> tablesPositions;
    [SerializeField] public List<FoodTypeSpritePair> foodSpritePairs;

    public Dictionary<FoodType, Sprite> foodSpriteDict = new();

    private float instantiateTime = 0f;
    private float timeToWaitForInstantiateNewClient = 5f;

    public Transform SpawnPosition { get => spawnPosition; }
    public Transform OutsidePosition { get => outsidePosition; }


    void Awake()
    {
        InitializeFoodSpriteDictionary();

        // Prueba
        ClientController client = clientPool.GetObjectFromPool<ClientController>();
    }

    void Update()
    {
        GetClientFromPool();
    }


    public Sprite GetSpriteForRandomFood(FoodType foodType)
    {
        foodSpriteDict.TryGetValue(foodType, out Sprite sprite);
        return sprite;
    }

    public void ReturnObjectToPool(ClientModel clientModel)
    {
        clientPool.ReturnObjectToPool(clientModel);
    }

    public Table GetRandomAvailableTable()
    {
        List<int> availableIndexes = new List<int>();

        for (int i = 0; i < tablesPositions.Count; i++)
        {
            if (!tablesPositions[i].IsOccupied)
            {
                availableIndexes.Add(i);
            }
        }

        int randomAvailableIndex = availableIndexes[Random.Range(0, availableIndexes.Count)];

        tablesPositions[randomAvailableIndex].IsOccupied = true;
        return tablesPositions[randomAvailableIndex];
    }

    public void FreeTable(Table tableToFree)
    {
        tableToFree.IsOccupied = false;
    }


    private void GetClientFromPool()
    {
        instantiateTime += Time.deltaTime;

        if (instantiateTime >= timeToWaitForInstantiateNewClient)
        {
            ClientController client = clientPool.GetObjectFromPool<ClientController>();
            instantiateTime = 0f;
        }
    }

    private void InitializeFoodSpriteDictionary()
    {
        foodSpriteDict.Clear();

        foreach (var pair in foodSpritePairs)
        {
            if (!foodSpriteDict.ContainsKey(pair.FoodType))
            {
                foodSpriteDict.Add(pair.FoodType, pair.Sprite);
            }
        }
    }

}

[System.Serializable]
public class FoodTypeSpritePair
{
    [SerializeField] private FoodType foodType;
    [SerializeField] private Sprite sprite;

    public FoodType FoodType { get => foodType; }
    public Sprite Sprite { get => sprite; }
}