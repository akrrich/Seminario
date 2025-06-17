using System.Collections.Generic;
using UnityEngine;

public class TablesManager : MonoBehaviour
{
    private static TablesManager instance;

    private List<Table> tables = new List<Table>();

    public static TablesManager Instance { get => instance; }

    public List<Table> Tables { get => tables; }


    void Awake()
    {
        CreateSingleton();
        InitializeTables();
    }


    public Table GetRandomAvailableTableForClient()
    {
        List<int> availableIndexes = new List<int>();

        for (int i = 0; i < tables.Count; i++)
        {
            if (!tables[i].IsOccupied)
            {
                availableIndexes.Add(i);
            }
        }

        if (availableIndexes.Count == 0) return null;

        int randomAvailableIndex = availableIndexes[Random.Range(0, availableIndexes.Count)];

        tables[randomAvailableIndex].IsOccupied = true;
        return tables[randomAvailableIndex];
    }

    // Hacer null a la fuerza a la table que iguale cuando llame al metodo
    public Table FreeTable(Table tableToFree)
    {
        if (tableToFree != null)
        {
            tableToFree.IsOccupied = false;
        }

        return null;
    }


    private void CreateSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(instance);
        }
    }

    /// <summary>
    /// Ajustar el metodo segun sea necesario en un futuro para agregar mesas cuando se desbloque zona o como se quiera
    /// </summary>
    private void InitializeTables()
    {
        GameObject[] tableObjects = GameObject.FindGameObjectsWithTag("Table");

        foreach (GameObject obj in tableObjects)
        {
            Table table = obj.GetComponentInParent<Table>();
            if (table != null)
            {
                tables.Add(table);
            }
        }
    }
}
