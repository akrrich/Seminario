using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class Table : MonoBehaviour
{
    private GameObject chair;
    private GameObject dish;
    private GameObject dirty;

    private NavMeshObstacle[] navMeshObstacle;

    private List<Transform> dishPositions = new List<Transform>(); // Representa las posiciones hijas del plato

    private List<Food> currentFoods = new List<Food>();

    private float currentCleanProgress = 0f;

    private bool isOccupied = false;
    private bool isDirty = false;

    public Transform ChairPosition { get => chair.transform; }
    public Transform DishPosition { get => dish.transform; } // Solamente para que mire hacia adelante que es esta posicion

    public List<Transform> DishPositions { get => dishPositions; }

    public List<Food> CurrentFoods { get => currentFoods; set => currentFoods = value; }

    public float CurrentCleanProgress { get => currentCleanProgress; set => currentCleanProgress = value; }
    
    public bool IsOccupied { get => isOccupied; set => isOccupied = value; }
    public bool IsDirty { get => isDirty; }


    void Awake()
    {
        FindObjectsAndComponents();
    }


    public void SetDirty(bool current)
    {
        isDirty = current;
        dirty.SetActive(current);
    }

    /// <summary>
    /// Analizar el metodo por el tema de el NavMesh del NPC
    /// </summary>
    public void SetNavMeshObstacles(bool current)
    {
        for (int i = 0; i < navMeshObstacle.Length; i++)
        {
            // No ejecutar si ya estaba activado y current es true, esto sirve por si se fue de la cola de espera porque no se libero ninguna silla
            if (navMeshObstacle[i].isActiveAndEnabled && current)
            {
                continue;
            }

            navMeshObstacle[i].enabled = current;
        }
    }


    private void FindObjectsAndComponents()
    {
        chair = transform.Find("Chair").gameObject;
        dish = transform.Find("Dish").gameObject;
        dirty = transform.Find("Dirty").gameObject;

        navMeshObstacle = GetComponentsInChildren<NavMeshObstacle>();

        foreach (Transform childs in dish.transform)
        {
            dishPositions.Add(childs.GetComponent<Transform>());
        }
    }
}
