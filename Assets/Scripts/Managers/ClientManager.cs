using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    [SerializeField] private ClientManagerData clientManagerData;

    [SerializeField] private Transform spawnPosition, outsidePosition;

    [SerializeField] private AbstractFactory clientAbstractFactory;
    [SerializeField] private List<ObjectPooler> clientPools;
    [SerializeField] private List<FoodTypeSpritePair> foodSpritePairs;

    private Dictionary<ClientType, ObjectPooler> clientPoolDictionary = new();
    private Dictionary<FoodType, Sprite> foodSpriteDict = new();

    private float spawnTime = 0f;

    [SerializeField] private bool InstantiateClients;
    [SerializeField] private bool InstantiateTheSameClient;

    public Transform SpawnPosition { get => spawnPosition; }
    public Transform OutsidePosition { get => outsidePosition; }


    void Awake()
    {
        InitializeClientPoolDictionary();
        InitializeFoodSpriteDictionary();

        // Provisorio
        if (InstantiateClients && !InstantiateTheSameClient)
        {
            StartCoroutine(InitializeRandomClient());
        }
    }

    void Update()
    {
        // Provisorio
        if (InstantiateTheSameClient)
        {
            GetTheSameClientFromPool();
        }

        else if (InstantiateClients)
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

        if (spawnTime >= clientManagerData.TimeToWaitForSpawnNewClient)
        {
            int randomIndex = UnityEngine.Random.Range(0, clientPools.Count);
            string prefabName = clientPools[randomIndex].Prefab.name;
            clientAbstractFactory.CreateObject(prefabName);
            
            spawnTime = 0f;
        }
    }

    // Provisorio
    private void GetTheSameClientFromPool()
    {
        spawnTime += Time.deltaTime;

        if (spawnTime > clientManagerData.TimeToWaitForSpawnNewClient)
        {
            clientAbstractFactory.CreateObject("ClientOgre");

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