using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public string poolName; // Tên pool để đăng ký
    public GameObject prefab;
    public int initialSize = 10;
    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        ObjectPoolManager.Instance.RegisterPool(poolName, this); // Đăng ký vào ObjectPoolManager

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = Instantiate(prefab);
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
