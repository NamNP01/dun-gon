using UnityEngine;

public class ArrowTrigger : MonoBehaviour
{
    public PlayerData playerData; // Lấy dữ liệu từ PlayerData
    private int enemyHitCount = 0; // Đếm số kẻ địch đã trúng

    private void OnTriggerEnter(Collider other)
    {
        // Nếu va vào tường thì hủy mũi tên
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
            return;
        }

        // Nếu va vào mục tiêu
        // Nếu va vào mục tiêu
        EnemyHP enemyHP = other.gameObject.GetComponent<EnemyHP>();
        if (enemyHP != null)
        {
            // Xác định xem có chí mạng hay không
            bool isCriticalHit = Random.value < playerData.critChance;
            int finalDamage = isCriticalHit ? Mathf.RoundToInt(playerData.Damage * playerData.critDamage) : playerData.Damage;

            // Nếu có Piercing Shot, giảm sát thương sau mỗi lần xuyên
            if (playerData.hasPiercingShot)
            {
                float damageMultiplier = Mathf.Pow(0.67f, enemyHitCount); // Mỗi lần xuyên giảm 33% (0.67^n)
                finalDamage = Mathf.RoundToInt(finalDamage * damageMultiplier);
                enemyHitCount++;
            }

            // Gây sát thương lên kẻ địch
            enemyHP.TakeDamage(finalDamage, isCriticalHit);

            // Nếu KHÔNG có Piercing Shot, hủy mũi tên sau khi bắn trúng
            if (!playerData.hasPiercingShot)
            {
                Destroy(gameObject);
            }
        }
    }
}