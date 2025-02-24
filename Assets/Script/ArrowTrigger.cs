using UnityEngine;

public class ArrowTrigger : MonoBehaviour
{
    public PlayerData playerData; // Lấy dữ liệu từ PlayerData

    private void OnTriggerEnter(Collider other)
    {
        // Nếu va vào tường thì hủy mũi tên
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
            return;
        }

        // Nếu va vào mục tiêu
        EnemyHP enemyHP = other.gameObject.GetComponent<EnemyHP>();
        if (enemyHP != null)
        {
            // Xác định xem có chí mạng hay không
            bool isCriticalHit = Random.value < playerData.critChance;
            int finalDamage = isCriticalHit ? Mathf.RoundToInt(playerData.Damage * playerData.critDamage) : playerData.Damage;

            // Gây sát thương lên kẻ địch
            enemyHP.TakeDamage(finalDamage, isCriticalHit);

            Destroy(gameObject); // Hủy mũi tên sau khi va chạm
        }
    }
}