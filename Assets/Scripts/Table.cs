using UnityEngine;
using System.Collections.Generic;

public class Table : MonoBehaviour
{
    private GameObject chair;
    private GameObject dish;

    private List<Transform> dishPositions = new List<Transform>(); // Representa las posiciones hijas del plato

    private List<Food> currentFoods = new List<Food>();

    private bool isOccupied = false;

    public Transform ChairPosition { get => chair.transform; }

    public List<Transform> DishPositions { get => dishPositions; }

    public List<Food> CurrentFoods { get => currentFoods; set => currentFoods = value; }

    public bool IsOccupied { get => isOccupied; set => isOccupied = value; }


    void Awake()
    {
        FindObjects();
    }


    private void FindObjects()
    {
        chair = transform.Find("Chair").gameObject;
        dish = transform.Find("Dish").gameObject;

        foreach (Transform childs in dish.transform)
        {
            dishPositions.Add(childs.GetComponent<Transform>());
        }
    }
}
