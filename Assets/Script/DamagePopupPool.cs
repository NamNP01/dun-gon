using System.Collections.Generic;
using UnityEngine;

public class DamagePopupPool : MonoBehaviour
{
    public static DamagePopupPool Instance;
    public GameObject popupPrefab;
    private Queue<GameObject> poolQueue = new Queue<GameObject>();
    public int initialPoolSize = 10; // 🔥 Số lượng popup tạo ban đầu

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 🔥 Khởi tạo trước 10 popup
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject popup = Instantiate(popupPrefab);
            popup.SetActive(false);
            poolQueue.Enqueue(popup);
        }
    }

    public GameObject GetPopup()
    {
        if (poolQueue.Count > 0)
        {
            GameObject popup = poolQueue.Dequeue();
            popup.SetActive(true);
            return popup;
        }
        else
        {
            return Instantiate(popupPrefab);
        }
    }

    public void ReturnPopup(GameObject popup)
    {
        popup.SetActive(false);
        poolQueue.Enqueue(popup);
    }
}
