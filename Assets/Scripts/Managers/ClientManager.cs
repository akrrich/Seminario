using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition, outsidePosition;

    [SerializeField] private ObjectPooler clientPool;
    [SerializeField] private List<Table> tablesPositions;

    private float instantiateTime = 0f;
    private float timeToWaitForInstantiateNewClient = 5f;

    public Transform SpawnPosition { get => spawnPosition; }
    public Transform OutsidePosition { get => outsidePosition; }


    void Awake()
    {
        ClientController client = clientPool.GetObjectFromPool<ClientController>();
    }

    void Update()
    {
        //InstantiateClient();
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


    private void InstantiateClient()
    {
        instantiateTime += Time.deltaTime;

        if (instantiateTime >= timeToWaitForInstantiateNewClient)
        {
            ClientController client = clientPool.GetObjectFromPool<ClientController>();
            instantiateTime = 0f;
        }
    }
}
