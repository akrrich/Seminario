using System.Collections.Generic;
using UnityEngine;

public class AbstractFactory : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    private Dictionary<string, GameObject> namePrefabs = new Dictionary<string, GameObject>();


    void Awake()
    {
        AddAllGameObjectsToDictionary();
    }

    void OnDestroy()
    {
        ClearAllGameObjectsFromDictionary();
    }


    // Opcional el Vector3 por si se quiere ajustar un poco la posicion
    public GameObject CreateObject(Transform transform, Vector3 position, ObjectPooler objectPool)
    {
        Vector3 newPosition = transform.position + position;

        MonoBehaviour pooledObject = objectPool.GetObjectFromPool<MonoBehaviour>();

        if (pooledObject == null)
        {
            return null;
        }

        pooledObject.transform.position = newPosition;

        return pooledObject.gameObject;
    }

    // Opcional el Vector3 por si se quiere ajustar un poco la posicion
    public GameObject CreateObject(string prefabName, Transform transform, Vector3 position, ObjectPooler objectPool)
    {
        if (!namePrefabs.TryGetValue(prefabName, out GameObject prefab))
        {
            return null;
        }

        Vector3 newPosition = transform.position + position;

        MonoBehaviour pooledObject = objectPool.GetObjectFromPool<MonoBehaviour>();
        prefab = pooledObject.gameObject;

        prefab.transform.position = newPosition;

        return prefab;
    }


    private void AddAllGameObjectsToDictionary()
    {
        foreach (var prefab in prefabs)
        {
            namePrefabs[prefab.name] = prefab;
        }
    }

    private void ClearAllGameObjectsFromDictionary()
    {
        namePrefabs.Clear();
    }
}
