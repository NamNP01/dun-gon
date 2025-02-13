using UnityEngine;

public class MonsterController : MonoBehaviour
{
    private RoomCondition roomCondition; // Lưu trữ RoomCondition của quái này

    void Start()
    {
        // Tìm RoomCondition gần nhất (từ cha hoặc ông)
        roomCondition = GetComponentInParent<RoomCondition>();

        if (roomCondition != null)
        {
            roomCondition.AddMonster(this.gameObject); // Thêm quái vào danh sách
        }
        else
        {
            Debug.LogError($"Monster {gameObject.name} không thuộc RoomCondition nào!");
        }
    }

    private void OnDestroy()
    {
        if (roomCondition != null)
        {
            roomCondition.RemoveMonster(this.gameObject); // Xóa quái khỏi danh sách khi bị hủy
        }
    }
}
