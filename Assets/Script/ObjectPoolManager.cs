using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;
    private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterPool(string poolName, ObjectPool pool)
    {
        if (!pools.ContainsKey(poolName))
        {
            pools[poolName] = pool;
        }
    }

    public GameObject GetObjectFromPool(string poolName, Vector3 position, Quaternion rotation)
    {
        if (pools.TryGetValue(poolName, out ObjectPool pool))
        {
            return pool.GetObject(position, rotation);
        }
        return null;
    }

    public void ReturnObjectToPool(string poolName, GameObject obj)
    {
        if (pools.TryGetValue(poolName, out ObjectPool pool))
        {
            pool.ReturnObject(obj);
        }
    }
}
