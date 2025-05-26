using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition, outsidePosition;

    [SerializeField] private AbstractFactory clientAbstractFactory;
    [SerializeField] private List<ObjectPooler> clientPools;
    [SerializeField] private List<FoodTypeSpritePair> foodSpritePairs;
    [SerializeField] private List<Table> tablesPositions;

    private Dictionary<ClientType, ObjectPooler> clientPoolDictionary = new();
    private Dictionary<FoodType, Sprite> foodSpriteDict = new();

    [SerializeField] private float timeToWaitForSpawnNewClient;
    private float spawnTime = 0f;

    [SerializeField] private bool InstantiateClients;

    public Transform SpawnPosition { get => spawnPosition; }
    public Transform OutsidePosition { get => outsidePosition; }


    void Awake()
    {
        InitializeClientPoolDictionary();
        InitializeFoodSpriteDictionary();

        StartCoroutine(InitializeRandomClient());
    }

    void Update()
    {
        // Provisorio
        if (InstantiateClients)
        {
            GetClientRandomFromPool();
        }
    }


    public Sprite GetSpriteForRandomFood(FoodType foodType)
    {
        foodSpriteDict.TryGetValue(foodType, out Sprite sprite);
        return sprite;
    }

    public void ReturnObjectToPool(ClientType clientType, ClientModel currentClient)
    {
        if (clientPoolDictionary.ContainsKey(clientType))
        {
            clientPoolDictionary[clientType].ReturnObjectToPool(currentClient);
        }
    }

    public void SetParentToHisPoolGameObject(ClientType clientType, ClientModel currentClient)
    {
        if (clientPoolDictionary.TryGetValue(clientType, out ObjectPooler pooler))
        {
            currentClient.transform.SetParent(pooler.transform);
        }
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

        if (availableIndexes.Count == 0) return null;

        int randomAvailableIndex = availableIndexes[UnityEngine.Random.Range(0, availableIndexes.Count)];

        tablesPositions[randomAvailableIndex].IsOccupied = true;
        return tablesPositions[randomAvailableIndex];
    }

    // Liberar unicamente la mesa si ya tenia asignada una
    public void FreeTable(Table tableToFree)
    {
        if (tableToFree != null)
        {
            tableToFree.IsOccupied = false;
        }
    }


    private IEnumerator InitializeRandomClient()
    {
        yield return new WaitUntil(() => clientPools.All(p => p != null && p.Prefab != null));

        int randomIndex = UnityEngine.Random.Range(0, clientPools.Count);
        string prefabName = clientPools[randomIndex].Prefab.name;
        clientAbstractFactory.CreateObject(prefabName);
    }

    private void GetClientRandomFromPool()
    {
        spawnTime += Time.deltaTime;

        if (spawnTime >= timeToWaitForSpawnNewClient)
        {
            int randomIndex = UnityEngine.Random.Range(0, clientPools.Count);
            string prefabName = clientPools[randomIndex].Prefab.name;
            clientAbstractFactory.CreateObject(prefabName);
            
            spawnTime = 0f;
        }
    }

    private void InitializeClientPoolDictionary()
    {
        for (int i = 0; i < clientPools.Count; i++)
        {
            if (Enum.IsDefined(typeof(ClientType), i))
            {
                ClientType foodType = (ClientType)i;
                clientPoolDictionary[foodType] = clientPools[i];
            }
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

[Serializable]
public class FoodTypeSpritePair
{
    [SerializeField] private FoodType foodType;
    [SerializeField] private Sprite sprite;

    public FoodType FoodType { get => foodType; }
    public Sprite Sprite { get => sprite; }
}