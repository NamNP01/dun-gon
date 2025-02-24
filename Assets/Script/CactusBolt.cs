using UnityEngine;

public class CactusProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int maxBounces = 3; // Số lần nảy tối đa
    public LayerMask bounceMask; // Layer của vật thể có thể nảy
    public int Damage;

    private int bounceCount = 0;
    private Vector3 direction;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // ✅ Chống đi xuyên tường
        rb.isKinematic = false;
        direction = transform.forward;
        rb.linearVelocity = direction * speed; // ✅ Di chuyển theo hướng bắn
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Chạm vào: " + other.gameObject.name); // 🛠 Debug kiểm tra va chạm

        if ((bounceMask.value & (1 << other.gameObject.layer)) != 0) // Nếu chạm vào vật thể có thể nảy
        {
            if (bounceCount < maxBounces)
            {
                // ✅ Sử dụng `ClosestPointOnBounds` thay vì `ClosestPoint` để tính mặt phẳng phản xạ chính xác hơn
                Vector3 normal = transform.position - other.ClosestPointOnBounds(transform.position);
                normal.Normalize();

                direction = Vector3.Reflect(direction, normal); // Phản xạ hướng bay
                rb.linearVelocity = direction * speed; // ✅ Cập nhật vận tốc mới
                bounceCount++;
            }
            else
            {
                Destroy(gameObject); // Nếu đã nảy đủ 3 lần thì biến mất
            }
        }
    }
}
