using UnityEngine;

public class Table : MonoBehaviour
{
    private GameObject chair;
    private GameObject table;
    private GameObject dish;


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
