using UnityEngine;
using System.Collections.Generic;

public class Table : MonoBehaviour
{
    private GameObject chair;
    private GameObject dish;
    private GameObject dirty;

    private List<Transform> dishPositions = new List<Transform>(); // Representa las posiciones hijas del plato

    private List<Food> currentFoods = new List<Food>();

    private bool isOccupied = false;
    [SerializeField] private bool isDirty = false;

    public Transform ChairPosition { get => chair.transform; }
    public GameObject Dirty { get => dirty; }

    public List<Transform> DishPositions { get => dishPositions; }

    public List<Food> CurrentFoods { get => currentFoods; set => currentFoods = value; }

    public bool IsOccupied { get => isOccupied; set => isOccupied = value; }
    public bool IsDirty { get => isDirty; set => isDirty = value; }


    void Awake()
    {
        FindObjects();
    }


    private void FindObjects()
    {
        chair = transform.Find("Chair").gameObject;
        dish = transform.Find("Dish").gameObject;
        dirty = transform.Find("Dirty").gameObject;

        foreach (Transform childs in dish.transform)
        {
            dishPositions.Add(childs.GetComponent<Transform>());
        }
    }
}
