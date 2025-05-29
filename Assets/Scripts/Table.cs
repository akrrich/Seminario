using UnityEngine;
using System.Collections.Generic;

public class Table : MonoBehaviour
{
    private GameObject chair;
    private GameObject dish;
    private GameObject dirty;

    //private SphereCollider sphereCollider;

    private List<Transform> dishPositions = new List<Transform>(); // Representa las posiciones hijas del plato

    private List<Food> currentFoods = new List<Food>();

    private float currentCleanProgress = 0f;

    private bool isOccupied = false;
    private bool isDirty = false;

    public Transform ChairPosition { get => chair.transform; }

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


    private void FindObjectsAndComponents()
    {
        chair = transform.Find("Chair").gameObject;
        dish = transform.Find("Dish").gameObject;
        dirty = transform.Find("Dirty").gameObject;

        //sphereCollider =  GetComponent<SphereCollider>();

        foreach (Transform childs in dish.transform)
        {
            dishPositions.Add(childs.GetComponent<Transform>());
        }
    }
}
