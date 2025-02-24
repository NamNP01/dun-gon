using UnityEngine;

public class CoinProjectile : MonoBehaviour
{
    public int Damage;
    private int hitCount = 0; // Biến đếm số lần va chạm
    public float rotationSpeed = 500f;
    void Update()
    {
        // Xoay liên tục quanh trục Y
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            hitCount++; // Tăng số lần va chạm

            if (hitCount >= 2) // Nếu va chạm lần 2 thì hủy
            {
                Destroy(gameObject);
            }
        }
    }
}
