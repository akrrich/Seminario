using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    [SerializeField] private GameObject clientPrefab;
    [SerializeField] private Transform spawnPosition, outsidePosition;

    [SerializeField] private List<Table> tablesPositions;

    private float instantiateTime = 0f;
    private float timeToWaitForInstantiateNewClient = 10f;

    public Transform OutsidePosition { get => outsidePosition; }


    void Awake()
    {
        Instantiate(clientPrefab, spawnPosition.position, Quaternion.identity, transform);  
    }

    void Update()
    {
        //InstantiateClient();
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
            Instantiate(clientPrefab, spawnPosition.position, Quaternion.identity, transform);
            instantiateTime = 0f;
        }
    }
}
