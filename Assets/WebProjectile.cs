using UnityEngine;

public class WebProjectile : MonoBehaviour
{
    public float speed = 10f;  // Tốc độ di chuyển
    public float lifetime = 5f; // Tự hủy sau 5 giây nếu không trúng gì
    public int Damage;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed; // ✅ Di chuyển theo hướng bắn

        Destroy(gameObject, lifetime); // 🔥 Hủy sau 5s nếu không va chạm
    }
}
