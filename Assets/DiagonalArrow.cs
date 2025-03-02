using UnityEngine;

public class DiagonalArrow : MonoBehaviour
{
    public float speed = 10f; // Tốc độ mũi tên
    private Vector3 moveDirection; // Hướng di chuyển cố định

    public void SetDirection(Vector3 direction)
    {
        moveDirection = direction.normalized; // Lưu hướng di chuyển
        moveDirection.y = 0; // Giữ nguyên độ cao (chỉ bay ngang)
        transform.rotation = Quaternion.LookRotation(moveDirection); // Xoay theo hướng bắn
        if (direction.sqrMagnitude > 0.001f) // Kiểm tra xem vector có khác (0,0,0) không
        {
            direction.Normalize();
            transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(-90, 0, 0);
        }
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime; // Di chuyển theo hướng đã đặt
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject); // Nếu chạm tường, hủy mũi tên
        }
    }
}
