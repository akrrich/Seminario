using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbstractFactory : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private List<ObjectPooler> objectsPools;

    private Dictionary<string, ObjectPooler> namePoolDictionary = new Dictionary<string, ObjectPooler>();


    void Awake()
    {
        AddAllGameObjectsToPoolDictionary();
    }

    void OnDestroy()
    {
        ClearAllGameObjectsFromDictionary();
    }

    // El Vector3 es opcional
    public GameObject CreateObject(string prefabName, Transform transform, Vector3 position)
    {
        if (!namePoolDictionary.TryGetValue(prefabName, out ObjectPooler pooler))
        {
            return null;
        }

        MonoBehaviour pooledObject = pooler.GetObjectFromPool<MonoBehaviour>();

        if (pooledObject == null)
        {
            return null;
        }

        pooledObject.transform.position = transform.position + position;
        return pooledObject.gameObject;
    }


    private void AddAllGameObjectsToPoolDictionary()
    {
        foreach (var prefab in prefabs)
        {
            ObjectPooler pooler = GetObjectPoolerForPrefab(prefab);
            namePoolDictionary[prefab.name] = pooler;
        }
    }

    private ObjectPooler GetObjectPoolerForPrefab(GameObject prefab)
    {
        return objectsPools.FirstOrDefault(pool => pool.Prefab == prefab);
    }

    private void ClearAllGameObjectsFromDictionary()
    {
        namePoolDictionary.Clear();
    }
}
