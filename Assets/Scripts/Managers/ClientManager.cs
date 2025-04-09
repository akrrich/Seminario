using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    [SerializeField] private GameObject clientPrefab;
    [SerializeField] private Transform spawnPosition, outsidePosition;

    [SerializeField] private List<Transform> tablesPositions;
    private List<bool> tablesOccupied = new List<bool>();

    private float instantiateTime = 0f;
    private float timeToWaitForInstantiateNewClient = 5f;

    public Transform OutsidePosition { get => outsidePosition; }


    void Awake()
    {
        InitializeTables();
        GameObject client = Instantiate(clientPrefab, spawnPosition.position, Quaternion.identity, transform);  
    }

    void Update()
    {
        //InstantiateClient();
    }


    public Transform GetRandomAvailableTable()
    {
        List<int> availableIndexes = new List<int>();

        for (int i = 0; i < tablesOccupied.Count; i++)
        {
            if (!tablesOccupied[i])
            {
                availableIndexes.Add(i);
            }
        }

        int randomAvailableIndex = availableIndexes[Random.Range(0, availableIndexes.Count)];

        tablesOccupied[randomAvailableIndex] = true;
        return tablesPositions[randomAvailableIndex];
    }

    public void FreeTable(Transform tableToFree)
    {
        int index = tablesPositions.IndexOf(tableToFree);
        if (index >= 0)
        {
            tablesOccupied[index] = false;
        }
    }


    private void InitializeTables()
    {
        for (int i = 0; i < tablesPositions.Count; i++)
        {
            tablesOccupied.Add(false);
        }
    }

    private void InstantiateClient()
    {
        instantiateTime += Time.deltaTime;

        if (instantiateTime >= timeToWaitForInstantiateNewClient)
        {
            GameObject client = Instantiate(clientPrefab, transform, spawnPosition);
            instantiateTime = 0f;
        }
    }
}
