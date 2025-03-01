using System.Collections.Generic;
using UnityEngine;

public class RoomCondition : MonoBehaviour
{
    public List<GameObject> MonsterListInRoom = new List<GameObject>();
    public bool playerInThisRoom = false;
    public bool isClearRoom = false;

    [Header("Đổi Material khi Clear Room")]
    public GameObject targetObject;  // GameObject cần đổi Material
    public Material clearedMaterial; // Material mới

    void Update()
    {
        if (playerInThisRoom && !isClearRoom && MonsterListInRoom.Count == 0)
        {
            isClearRoom = true;
            Debug.Log("Clear Room!");

            // Xóa danh sách quái trong PlayerTargeting
            if (PlayerTargeting.Instance != null)
            {
                PlayerTargeting.Instance.MonsterList.Clear();
                Debug.Log("Target List Cleared!");
            }

            // Đổi Material nếu có targetObject và Material mới
            if (targetObject != null && clearedMaterial != null)
            {
                Renderer rend = targetObject.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material = clearedMaterial;
                    Debug.Log("Đã đổi Material của " + targetObject.name);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInThisRoom = true;

            // Cập nhật danh sách quái cho PlayerTargeting
            if (PlayerTargeting.Instance != null)
            {
                PlayerTargeting.Instance.MonsterList = new List<GameObject>(MonsterListInRoom);
                Debug.Log("Enter New Room! Mob Count: " + PlayerTargeting.Instance.MonsterList.Count);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInThisRoom = false;

            if (PlayerTargeting.Instance != null)
            {
                PlayerTargeting.Instance.MonsterList.Clear();
            }

            Debug.Log("Player Exit!");
        }
    }

    public void RemoveMonster(GameObject monster)
    {
        if (MonsterListInRoom.Contains(monster))
        {
            MonsterListInRoom.Remove(monster);
            Debug.Log("Monster Removed: " + monster.name);
        }
    }

    public void AddMonster(GameObject monster)
    {
        if (!MonsterListInRoom.Contains(monster))
        {
            MonsterListInRoom.Add(monster);
            Debug.Log($"Thêm quái: {monster.name} vào {gameObject.name}");
            if (PlayerTargeting.Instance != null)
            {
                PlayerTargeting.Instance.MonsterList = new List<GameObject>(MonsterListInRoom);
                Debug.Log("Enter New Room! Mob Count: " + PlayerTargeting.Instance.MonsterList.Count);
            }
        }
    }
}
