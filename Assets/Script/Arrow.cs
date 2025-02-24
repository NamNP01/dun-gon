using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 10f; // Tốc độ bay của mũi tên
    private Vector3 moveDirection; // Hướng bay cố định
    //public PlayerData playerData; // Lấy dữ liệu từ PlayerData
    private float initialY; // Độ cao ban đầu

    public void SetTarget(GameObject newTarget)
    {
        if (newTarget != null)
        {
            // Tính hướng bắn một lần duy nhất
            moveDirection = (newTarget.transform.position - transform.position).normalized;
            moveDirection.y = 0; // Giữ nguyên độ cao (chỉ bay ngang)
            Vector3 direction = newTarget.transform.position - transform.position;
            if (direction.sqrMagnitude > 0.001f) // Kiểm tra xem vector có khác (0,0,0) không
            {
                direction.Normalize();
                transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(-90, 0, 0);
            }
        }
        else
        {
            moveDirection = transform.forward; // Nếu không có mục tiêu, bắn thẳng
        }

        // Lưu lại độ cao ban đầu
        initialY = transform.position.y;
    }

    void Update()
    {

        // Di chuyển mũi tên theo hướng đã tính
        transform.position += moveDirection * speed * Time.deltaTime;


        // Giữ nguyên độ cao ban đầu
        transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Nếu va vào tường thì hủy mũi tên
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
            return;
        }

        //// Nếu va vào mục tiêu
        //EnemyHP enemyHP = other.gameObject.GetComponent<EnemyHP>();
        //if (enemyHP != null)
        //{
        //    // Xác định xem có chí mạng hay không
        //    bool isCriticalHit = Random.value < playerData.critChance;
        //    int finalDamage = isCriticalHit ? Mathf.RoundToInt(playerData.Damage * playerData.critDamage) : playerData.Damage;

        //    // Gây sát thương lên kẻ địch
        //    enemyHP.TakeDamage(finalDamage, isCriticalHit);

        //    Destroy(gameObject); // Hủy mũi tên sau khi va chạm
        //}
    }
}
