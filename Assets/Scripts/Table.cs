using UnityEngine;

public class Table : MonoBehaviour
{
    private GameObject chair;
    private GameObject table;
    private GameObject dish;

    private Food currentFood;

    private bool isOccupied = false;

    public Transform ChairPosition { get => chair.transform; }
    public Transform DishPosition { get => dish.transform; }

    public Food CurrentFood { get => currentFood; set => currentFood = value; }

    public bool IsOccupied { get => isOccupied; set => isOccupied = value; }


    void Awake()
    {
        FindObjects();
    }


    private void FindObjects()
    {
        chair = transform.Find("Chair").gameObject;
        table = transform.Find("Table").gameObject;
        dish = transform.Find("Dish").gameObject;
    }
}
