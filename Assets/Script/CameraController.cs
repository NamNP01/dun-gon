using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; // Nhân vật
    public Vector2 stepSize = new Vector2(20f, 30f); // Kích thước mỗi bước nhảy (X, Z)

    public void MoveToNearestStage()
    {

        if (player == null)
        {
            return;
        }

        // Lấy vị trí hiện tại của nhân vật
        Vector3 playerPos = player.position;

        // Tìm vị trí gần nhất theo stepSize
        float nearestX = Mathf.Round(playerPos.x / stepSize.x) * stepSize.x;
        float nearestZ = Mathf.Round(playerPos.z / stepSize.y) * stepSize.y;

        // Dịch chuyển Camera
        Vector3 newPosition = new Vector3(nearestX, transform.position.y, nearestZ);
        transform.position = newPosition;
    }

}
