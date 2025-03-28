using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize;

    private Queue<MonoBehaviour> pool = new Queue<MonoBehaviour>();

    public GameObject Prefab { get => prefab; }


    void Awake()
    {
        InitializePool();
    }


    public T GetObjectFromPool<T>() where T : MonoBehaviour
    {
        if (pool.Count > 0)
        {
            MonoBehaviour obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj as T;
        }

        else
        {
            GameObject newObj = Instantiate(prefab, transform);
            return newObj.GetComponent<T>();
        }
    }

    public void ReturnObjectToPool(MonoBehaviour obj)
    {
        obj.gameObject.transform.SetParent(transform);
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }


    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj.GetComponent<MonoBehaviour>());
        }
    }
}
