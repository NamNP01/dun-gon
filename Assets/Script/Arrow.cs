using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 10f; // Tốc độ bay của mũi tên
    private GameObject target;
    public PlayerData playerData; // Lấy dữ liệu từ PlayerData

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Tính hướng di chuyển đến mục tiêu
        Vector3 direction = target.transform.position - transform.position;
        if (direction.sqrMagnitude > 0.001f) // Kiểm tra xem vector có khác (0,0,0) không
        {
            direction.Normalize();
            transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(-90, 0, 0);
        }

        // Di chuyển về phía mục tiêu
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target != null && other.gameObject == target)
        {
            EnemyHP enemyHP = target.GetComponent<EnemyHP>();
            if (enemyHP != null)
            {
                // Xác định xem có chí mạng hay không
                bool isCriticalHit = Random.value < playerData.critChance;
                int finalDamage = isCriticalHit ? Mathf.RoundToInt(playerData.Damage * playerData.critDamage) : playerData.Damage;

                //// In ra log để kiểm tra
                //if (isCriticalHit)
                //{
                //    Debug.Log("Critical Hit! Damage: " + finalDamage);
                //}
                //else
                //{
                //    Debug.Log("Normal Hit! Damage: " + finalDamage);
                //}

                // Gây sát thương lên kẻ địch
                enemyHP.TakeDamage(finalDamage, isCriticalHit);

                // Đẩy lùi kẻ địch
                Rigidbody enemyRb = target.GetComponent<Rigidbody>();
                if (enemyRb != null && enemyRb.isKinematic)
                {
                    Vector3 knockbackDirection = (target.transform.position - transform.position).normalized;
                    float knockbackDistance = 0.2f; // Giảm khoảng cách đẩy lùi
                    Vector3 newPosition = target.transform.position + knockbackDirection * knockbackDistance;

                    // Dùng MovePosition thay vì thay đổi transform.position
                    enemyRb.MovePosition(newPosition);
                }
            }

            Destroy(gameObject);
        }
    }
}
